using Moq;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Common.Filtering.Filters;
using WHMS.Application.Features.Command.Delivery.DeleteDelivery;
using WHMS.Domain.Entities;
using WHMS.Domain.Exceptions;

namespace WHMS.Tests.Application.Features.Command.Delivery.DeleteDelivery;

// Note: the handler class itself is named "DeleteDeliveryCommandHnadler" (typo) in
// the source - referenced here as-is, not renamed.
public class DeleteDeliveryCommandHandlerTests
{
    private readonly Mock<IDeliveryRepository> _deliveryRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly DeleteDeliveryCommandHnadler _handler;
    private readonly Guid _deliveryId = Guid.NewGuid();

    public DeleteDeliveryCommandHandlerTests()
    {
        _handler = new DeleteDeliveryCommandHnadler(_deliveryRepository.Object, _unitOfWork.Object);
    }

    private DeleteDeliveryCommandRequest Request() => new() { DeliveryId = _deliveryId.ToString() };

    [Fact]
    public async Task Handle_DeliveryNotFound_ThrowsAndDoesNotCommit()
    {
        _deliveryRepository.Setup(r => r.GetDelivery(_deliveryId)).ReturnsAsync((WHMS.Domain.Entities.Delivery)null!);

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(Request(), CancellationToken.None));

        Assert.Equal("Delivery not found.", exception.Message);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_DeliveryAlreadyReceived_ThrowsAndDoesNotDelete()
    {
        var delivery = new WHMS.Domain.Entities.Delivery { Id = _deliveryId, ReceivedAt = DateTime.Now };
        _deliveryRepository.Setup(r => r.GetDelivery(_deliveryId)).ReturnsAsync(delivery);

        var exception = await Assert.ThrowsAsync<BusinessRuleViolationException>(() => _handler.Handle(Request(), CancellationToken.None));

        Assert.Equal("Delivery has already been received.", exception.Message);
        _deliveryRepository.Verify(r => r.DeleteDelivery(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task Handle_NotReceived_DeletesAllItemsThenTheDeliveryItself()
    {
        var delivery = new WHMS.Domain.Entities.Delivery { Id = _deliveryId, ReceivedAt = null };
        var item1 = new DeliveryItem { Id = Guid.NewGuid(), DeliveryId = _deliveryId, SkuId = Guid.NewGuid(), Quantity = 1 };
        var item2 = new DeliveryItem { Id = Guid.NewGuid(), DeliveryId = _deliveryId, SkuId = Guid.NewGuid(), Quantity = 2 };
        _deliveryRepository.Setup(r => r.GetDelivery(_deliveryId)).ReturnsAsync(delivery);
        _deliveryRepository
            .Setup(r => r.GetDeliveryItems(It.Is<DeliveryItemFilter>(f => f.DeliveryId == _deliveryId.ToString()), false))
            .ReturnsAsync([item1, item2]);

        await _handler.Handle(Request(), CancellationToken.None);

        _deliveryRepository.Verify(r => r.DeleteDeliveryItem(item1.Id), Times.Once);
        _deliveryRepository.Verify(r => r.DeleteDeliveryItem(item2.Id), Times.Once);
        _deliveryRepository.Verify(r => r.DeleteDelivery(_deliveryId), Times.Once);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
    }
}
