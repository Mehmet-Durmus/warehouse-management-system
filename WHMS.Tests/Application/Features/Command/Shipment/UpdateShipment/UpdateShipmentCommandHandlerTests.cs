using Moq;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Features.Command.Shipment.UpdateShipment;

namespace WHMS.Tests.Application.Features.Command.Shipment.UpdateShipment;

public class UpdateShipmentCommandHandlerTests
{
    private readonly Mock<IShipmentRepository> _shipmentRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IWarehouseRepository> _warehouseRepository = new();
    private readonly Mock<IStoreRepository> _storeRepository = new();
    private readonly UpdateShipmentCommandHandler _handler;
    private readonly Guid _shipmentId = Guid.NewGuid();
    private readonly Guid _warehouseId = Guid.NewGuid();
    private readonly Guid _storeId = Guid.NewGuid();

    public UpdateShipmentCommandHandlerTests()
    {
        _handler = new UpdateShipmentCommandHandler(_shipmentRepository.Object, _unitOfWork.Object, _warehouseRepository.Object, _storeRepository.Object);
    }

    private UpdateShipmentCommandRequest Request() => new()
    {
        ShipmentId = _shipmentId.ToString(),
        WarehouseId = _warehouseId.ToString(),
        StoreId = _storeId.ToString(),
        ExpectedSendingDate = new DateTime(2026, 3, 1)
    };

    [Fact]
    public async Task Handle_ShipmentNotFound_ThrowsAndDoesNotCommit()
    {
        _shipmentRepository.Setup(r => r.GetShipment(_shipmentId)).ReturnsAsync((WHMS.Domain.Entities.Shipment)null!);

        var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(Request(), CancellationToken.None));

        Assert.Equal("Shipment not found.", exception.Message);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_ShipmentAlreadySent_ThrowsWithoutCheckingWarehouseOrStore()
    {
        var shipment = new WHMS.Domain.Entities.Shipment { Id = _shipmentId, SendingDate = DateTime.Now };
        _shipmentRepository.Setup(r => r.GetShipment(_shipmentId)).ReturnsAsync(shipment);

        var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(Request(), CancellationToken.None));

        Assert.Equal("Shipment has already been sent.", exception.Message);
        _warehouseRepository.Verify(r => r.GetWarehouse(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WarehouseNotFound_ThrowsWithoutCheckingStore()
    {
        var shipment = new WHMS.Domain.Entities.Shipment { Id = _shipmentId, SendingDate = null };
        _shipmentRepository.Setup(r => r.GetShipment(_shipmentId)).ReturnsAsync(shipment);
        _warehouseRepository.Setup(r => r.GetWarehouse(_warehouseId)).ReturnsAsync((WHMS.Domain.Entities.Warehouse)null!);

        var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(Request(), CancellationToken.None));

        Assert.Equal("Warehouse not found.", exception.Message);
        _storeRepository.Verify(r => r.GetStore(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task Handle_StoreNotFound_ThrowsAndLeavesShipmentUnchanged()
    {
        var shipment = new WHMS.Domain.Entities.Shipment { Id = _shipmentId, WarehouseId = Guid.NewGuid(), SendingDate = null };
        var originalWarehouseId = shipment.WarehouseId;
        _shipmentRepository.Setup(r => r.GetShipment(_shipmentId)).ReturnsAsync(shipment);
        _warehouseRepository.Setup(r => r.GetWarehouse(_warehouseId)).ReturnsAsync(new WHMS.Domain.Entities.Warehouse
        {
            Id = _warehouseId, WarehouseName = "Depo", NormalizedName = "DEPO",
            Address = new WHMS.Domain.ValueObjects.Address(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "34000", "Cadde")
        });
        _storeRepository.Setup(r => r.GetStore(_storeId)).ReturnsAsync((WHMS.Domain.Entities.Store)null!);

        var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(Request(), CancellationToken.None));

        Assert.Equal("Store not found.", exception.Message);
        Assert.Equal(originalWarehouseId, shipment.WarehouseId);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_ValidUpdate_UpdatesShipmentAndCommits()
    {
        var shipment = new WHMS.Domain.Entities.Shipment { Id = _shipmentId, WarehouseId = Guid.NewGuid(), StoreId = Guid.NewGuid(), SendingDate = null };
        _shipmentRepository.Setup(r => r.GetShipment(_shipmentId)).ReturnsAsync(shipment);
        _warehouseRepository.Setup(r => r.GetWarehouse(_warehouseId)).ReturnsAsync(new WHMS.Domain.Entities.Warehouse
        {
            Id = _warehouseId, WarehouseName = "Depo", NormalizedName = "DEPO",
            Address = new WHMS.Domain.ValueObjects.Address(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "34000", "Cadde")
        });
        _storeRepository.Setup(r => r.GetStore(_storeId)).ReturnsAsync(new WHMS.Domain.Entities.Store
        {
            Id = _storeId, StoreName = "Magaza", NormalizedName = "MAGAZA",
            Address = new WHMS.Domain.ValueObjects.Address(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "34000", "Cadde")
        });

        var response = await _handler.Handle(Request(), CancellationToken.None);

        Assert.Equal(_warehouseId, shipment.WarehouseId);
        Assert.Equal(_storeId, shipment.StoreId);
        _shipmentRepository.Verify(r => r.UpdateShipment(shipment), Times.Once);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        Assert.Equal(_warehouseId.ToString(), response.WarehouseId);
        Assert.Equal(_storeId.ToString(), response.StoreId);
    }
}
