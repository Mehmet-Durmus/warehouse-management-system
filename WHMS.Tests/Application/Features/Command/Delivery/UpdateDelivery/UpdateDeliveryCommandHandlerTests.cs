using Moq;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Features.Command.Delivery.UpdateDelivery;
using WHMS.Domain.Exceptions;

namespace WHMS.Tests.Application.Features.Command.Delivery.UpdateDelivery;

public class UpdateDeliveryCommandHandlerTests
{
    private readonly Mock<IDeliveryRepository> _deliveryRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IWarehouseRepository> _warehouseRepository = new();
    private readonly UpdateDeliveryCommandHandler _handler;
    private readonly Guid _deliveryId = Guid.NewGuid();
    private readonly Guid _warehouseId = Guid.NewGuid();

    public UpdateDeliveryCommandHandlerTests()
    {
        _handler = new UpdateDeliveryCommandHandler(_deliveryRepository.Object, _unitOfWork.Object, _warehouseRepository.Object);
    }

    private UpdateDeliveryCommandRequest Request(DateTime date) => new()
    {
        DeliveryId = _deliveryId.ToString(),
        WarehouseId = _warehouseId.ToString(),
        ExpectedArrivalDate = date
    };

    [Fact]
    public async Task Handle_DeliveryNotFound_ThrowsAndDoesNotCommit()
    {
        _deliveryRepository.Setup(r => r.GetDelivery(_deliveryId)).ReturnsAsync((WHMS.Domain.Entities.Delivery)null!);

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(Request(DateTime.Today), CancellationToken.None));

        Assert.Equal("Delivery not found.", exception.Message);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_DeliveryAlreadyReceived_ThrowsWithoutCheckingWarehouse()
    {
        var delivery = new WHMS.Domain.Entities.Delivery { Id = _deliveryId, ReceivedAt = DateTime.Now };
        _deliveryRepository.Setup(r => r.GetDelivery(_deliveryId)).ReturnsAsync(delivery);

        var exception = await Assert.ThrowsAsync<BusinessRuleViolationException>(() => _handler.Handle(Request(DateTime.Today), CancellationToken.None));

        Assert.Equal("Delivery has already been received.", exception.Message);
        _warehouseRepository.Verify(r => r.GetWarehouse(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WarehouseNotFound_ThrowsAndLeavesDeliveryUnchanged()
    {
        var delivery = new WHMS.Domain.Entities.Delivery { Id = _deliveryId, WarehouseId = Guid.NewGuid(), ReceivedAt = null };
        var originalWarehouseId = delivery.WarehouseId;
        _deliveryRepository.Setup(r => r.GetDelivery(_deliveryId)).ReturnsAsync(delivery);
        _warehouseRepository.Setup(r => r.GetWarehouse(_warehouseId)).ReturnsAsync((WHMS.Domain.Entities.Warehouse)null!);

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(Request(DateTime.Today), CancellationToken.None));

        Assert.Equal("Warehouse not found.", exception.Message);
        Assert.Equal(originalWarehouseId, delivery.WarehouseId);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_ValidUpdate_UpdatesDeliveryAndCommits()
    {
        var delivery = new WHMS.Domain.Entities.Delivery { Id = _deliveryId, WarehouseId = Guid.NewGuid(), ReceivedAt = null };
        var warehouse = new WHMS.Domain.Entities.Warehouse
        {
            Id = _warehouseId,
            WarehouseName = "Yeni Depo",
            NormalizedName = "YENI DEPO",
            Address = new WHMS.Domain.ValueObjects.Address(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "34000", "Cadde")
        };
        _deliveryRepository.Setup(r => r.GetDelivery(_deliveryId)).ReturnsAsync(delivery);
        _warehouseRepository.Setup(r => r.GetWarehouse(_warehouseId)).ReturnsAsync(warehouse);
        var newDate = new DateTime(2026, 2, 1);

        var response = await _handler.Handle(Request(newDate), CancellationToken.None);

        Assert.Equal(_warehouseId, delivery.WarehouseId);
        Assert.Equal(newDate, delivery.ExpectedArrivalDate);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        Assert.Equal("Yeni Depo", response.WarehouseName);
        Assert.Equal(newDate, response.ExpectedArrivalDate);
    }
}
