using Moq;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Features.Command.Delivery.DeleteDeliveryItem;
using WHMS.Domain.Entities;
using WHMS.Domain.Exceptions;

namespace WHMS.Tests.Application.Features.Command.Delivery.DeleteDeliveryItem;

public class DeleteDeliveryItemCommandHandlerTests
{
    private readonly Mock<IDeliveryRepository> _deliveryRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly DeleteDeliveryItemCommandHandler _handler;
    private readonly Guid _deliveryItemId = Guid.NewGuid();
    private readonly Guid _deliveryId = Guid.NewGuid();

    public DeleteDeliveryItemCommandHandlerTests()
    {
        _handler = new DeleteDeliveryItemCommandHandler(_deliveryRepository.Object, _unitOfWork.Object);
    }

    private DeleteDeliveryItemCommandRequest Request() => new() { DeliveryItemId = _deliveryItemId.ToString() };

    [Fact]
    public async Task Handle_DeliveryItemNotFound_ThrowsAndDoesNotCommit()
    {
        _deliveryRepository.Setup(r => r.GetDeliveryItem(_deliveryItemId)).ReturnsAsync((DeliveryItem)null!);

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(Request(), CancellationToken.None));

        Assert.Equal("Delivery item not found.", exception.Message);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_ParentDeliveryNotFound_ThrowsAndDoesNotCommit()
    {
        var item = new DeliveryItem { Id = _deliveryItemId, DeliveryId = _deliveryId, SkuId = Guid.NewGuid(), Quantity = 1 };
        _deliveryRepository.Setup(r => r.GetDeliveryItem(_deliveryItemId)).ReturnsAsync(item);
        _deliveryRepository.Setup(r => r.GetDelivery(_deliveryId)).ReturnsAsync((WHMS.Domain.Entities.Delivery)null!);

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(Request(), CancellationToken.None));

        Assert.Equal("Delivery not found.", exception.Message);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_ParentDeliveryAlreadyReceived_ThrowsAndDoesNotDelete()
    {
        var item = new DeliveryItem { Id = _deliveryItemId, DeliveryId = _deliveryId, SkuId = Guid.NewGuid(), Quantity = 1 };
        var delivery = new WHMS.Domain.Entities.Delivery { Id = _deliveryId, ReceivedAt = DateTime.Now };
        _deliveryRepository.Setup(r => r.GetDeliveryItem(_deliveryItemId)).ReturnsAsync(item);
        _deliveryRepository.Setup(r => r.GetDelivery(_deliveryId)).ReturnsAsync(delivery);

        var exception = await Assert.ThrowsAsync<BusinessRuleViolationException>(() => _handler.Handle(Request(), CancellationToken.None));

        Assert.Equal("Delivery has already been received.", exception.Message);
        _deliveryRepository.Verify(r => r.DeleteDeliveryItem(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task Handle_DeliveryNotYetReceived_DeletesItemAndCommits()
    {
        var item = new DeliveryItem { Id = _deliveryItemId, DeliveryId = _deliveryId, SkuId = Guid.NewGuid(), Quantity = 1 };
        var delivery = new WHMS.Domain.Entities.Delivery { Id = _deliveryId, ReceivedAt = null };
        _deliveryRepository.Setup(r => r.GetDeliveryItem(_deliveryItemId)).ReturnsAsync(item);
        _deliveryRepository.Setup(r => r.GetDelivery(_deliveryId)).ReturnsAsync(delivery);

        await _handler.Handle(Request(), CancellationToken.None);

        _deliveryRepository.Verify(r => r.DeleteDeliveryItem(_deliveryItemId), Times.Once);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
    }
}
