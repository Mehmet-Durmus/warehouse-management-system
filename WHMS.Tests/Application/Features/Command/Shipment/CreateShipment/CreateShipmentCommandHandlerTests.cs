using Moq;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Features.Command.Shipment.CreateShipment;
using WHMS.Domain.Exceptions;

namespace WHMS.Tests.Application.Features.Command.Shipment.CreateShipment;

public class CreateShipmentCommandHandlerTests
{
    private readonly Mock<IShipmentRepository> _shipmentRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IWarehouseRepository> _warehouseRepository = new();
    private readonly Mock<IStoreRepository> _storeRepository = new();
    private readonly CreateShipmentCommandHandler _handler;
    private readonly Guid _warehouseId = Guid.NewGuid();
    private readonly Guid _storeId = Guid.NewGuid();

    public CreateShipmentCommandHandlerTests()
    {
        _handler = new CreateShipmentCommandHandler(_shipmentRepository.Object, _unitOfWork.Object, _warehouseRepository.Object, _storeRepository.Object);
    }

    private CreateShipmentCommandRequest Request() => new()
    {
        WarehouseId = _warehouseId.ToString(),
        StoreId = _storeId.ToString(),
        ExpectedSendingDate = new DateTime(2026, 3, 1)
    };

    [Fact]
    public async Task Handle_WarehouseNotFound_ThrowsWithoutCheckingStore()
    {
        _warehouseRepository.Setup(r => r.GetWarehouse(_warehouseId)).ReturnsAsync((WHMS.Domain.Entities.Warehouse)null!);

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(Request(), CancellationToken.None));

        // Message now includes a trailing period, sourced from the shared WarehouseRules.EnsureExists.
        Assert.Equal("Warehouse not found.", exception.Message);
        _storeRepository.Verify(r => r.GetStore(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task Handle_StoreNotFound_ThrowsAndDoesNotCreate()
    {
        _warehouseRepository.Setup(r => r.GetWarehouse(_warehouseId)).ReturnsAsync(new WHMS.Domain.Entities.Warehouse
        {
            Id = _warehouseId, WarehouseName = "Depo", NormalizedName = "DEPO",
            Address = new WHMS.Domain.ValueObjects.Address(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "34000", "Cadde")
        });
        _storeRepository.Setup(r => r.GetStore(_storeId)).ReturnsAsync((WHMS.Domain.Entities.Store)null!);

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(Request(), CancellationToken.None));

        Assert.Equal("Store not found.", exception.Message);
        _shipmentRepository.Verify(r => r.CreateShipment(It.IsAny<WHMS.Domain.Entities.Shipment>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ValidRequest_CreatesShipmentAndReturnsResponse()
    {
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

        _shipmentRepository.Verify(r => r.CreateShipment(It.Is<WHMS.Domain.Entities.Shipment>(s =>
            s.WarehouseId == _warehouseId && s.StoreId == _storeId)), Times.Once);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        Assert.Equal(_warehouseId.ToString(), response.WarehouseId);
        Assert.Equal(_storeId.ToString(), response.StoreId);
    }
}
