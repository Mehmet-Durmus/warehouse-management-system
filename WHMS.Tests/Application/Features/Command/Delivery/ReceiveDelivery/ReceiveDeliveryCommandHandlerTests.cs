using Moq;
using WHMS.Application.Abstractions.Infrastructure;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Features.Command.Delivery.ReceiveDelivery;
using WHMS.Domain.Entities;
using WHMS.Domain.Exceptions;

namespace WHMS.Tests.Application.Features.Command.Delivery.ReceiveDelivery;

public class ReceiveDeliveryCommandHandlerTests
{
    private readonly Mock<IDeliveryRepository> _deliveryRepository = new();
    private readonly Mock<IStockStateRepository> _stockStateRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<ICurrentUserService> _currentUserService = new();
    private readonly ReceiveDeliveryCommandHandler _handler;
    private readonly Guid _deliveryId = Guid.NewGuid();
    private readonly Guid _warehouseId = Guid.NewGuid();
    private readonly Guid _currentUserId = Guid.NewGuid();

    public ReceiveDeliveryCommandHandlerTests()
    {
        _currentUserService.Setup(u => u.WarehouseId).Returns(_warehouseId.ToString());
        _currentUserService.Setup(u => u.UserId).Returns(_currentUserId);
        _handler = new ReceiveDeliveryCommandHandler(_deliveryRepository.Object, _stockStateRepository.Object, _unitOfWork.Object, _currentUserService.Object);
    }

    private ReceiveDeliveryCommandRequest Request() => new() { DeliveryId = _deliveryId.ToString() };

    [Fact]
    public async Task Handle_DeliveryNotFound_ThrowsAndDoesNotCommit()
    {
        _deliveryRepository.Setup(r => r.GetDelivery(_deliveryId)).ReturnsAsync((WHMS.Domain.Entities.Delivery)null!);

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(Request(), CancellationToken.None));

        Assert.Equal("Delivery not found", exception.Message);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_DeliveryBelongsToAnotherWarehouse_ThrowsSameNotFoundMessage()
    {
        var delivery = new WHMS.Domain.Entities.Delivery { Id = _deliveryId, WarehouseId = Guid.NewGuid() };
        _deliveryRepository.Setup(r => r.GetDelivery(_deliveryId)).ReturnsAsync(delivery);

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(Request(), CancellationToken.None));

        Assert.Equal("Delivery not found", exception.Message);
    }

    [Fact]
    public async Task Handle_DeliveryAlreadyReceived_ThrowsAndDoesNotUpdateStock()
    {
        var delivery = new WHMS.Domain.Entities.Delivery { Id = _deliveryId, WarehouseId = _warehouseId, ReceivedAt = DateTime.Now };
        _deliveryRepository.Setup(r => r.GetDelivery(_deliveryId)).ReturnsAsync(delivery);

        var exception = await Assert.ThrowsAsync<BusinessRuleViolationException>(() => _handler.Handle(Request(), CancellationToken.None));

        Assert.Equal("Delivery already received", exception.Message);
        _stockStateRepository.Verify(r => r.UpdateQuantity(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ValidWithItems_IncreasesStockForEachItemAndMarksReceived()
    {
        var item1 = new DeliveryItem { Id = Guid.NewGuid(), SkuId = Guid.NewGuid(), Quantity = 5 };
        var item2 = new DeliveryItem { Id = Guid.NewGuid(), SkuId = Guid.NewGuid(), Quantity = 3 };
        var delivery = new WHMS.Domain.Entities.Delivery
        {
            Id = _deliveryId,
            WarehouseId = _warehouseId,
            ReceivedAt = null,
            DeliveryItems = [item1, item2]
        };
        _deliveryRepository.Setup(r => r.GetDelivery(_deliveryId)).ReturnsAsync(delivery);

        await _handler.Handle(Request(), CancellationToken.None);

        Assert.NotNull(delivery.ReceivedAt);
        Assert.Equal(_currentUserId, delivery.ReceivedById);
        _stockStateRepository.Verify(r => r.UpdateQuantity(_warehouseId, item1.SkuId, 5), Times.Once);
        _stockStateRepository.Verify(r => r.UpdateQuantity(_warehouseId, item2.SkuId, 3), Times.Once);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidWithNoItems_MarksReceivedWithoutTouchingStock()
    {
        var delivery = new WHMS.Domain.Entities.Delivery { Id = _deliveryId, WarehouseId = _warehouseId, ReceivedAt = null, DeliveryItems = null };
        _deliveryRepository.Setup(r => r.GetDelivery(_deliveryId)).ReturnsAsync(delivery);

        await _handler.Handle(Request(), CancellationToken.None);

        Assert.NotNull(delivery.ReceivedAt);
        _stockStateRepository.Verify(r => r.UpdateQuantity(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<int>()), Times.Never);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
    }
}
