using Moq;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Common.Filtering.Filters;
using WHMS.Application.Features.Command.Shipment.UpdateShipmentItem;
using WHMS.Domain.Entities;
using WHMS.Domain.Exceptions;

namespace WHMS.Tests.Application.Features.Command.Shipment.UpdateShipmentItem;

public class UpdateShipmentItemCommandHandlerTests
{
    private readonly Mock<IShipmentRepository> _shipmentRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<ICatalogRepository> _catalogRepository = new();
    private readonly Mock<IDeliveryRepository> _deliveryRepository = new();
    private readonly Mock<IStockStateRepository> _stockStateRepository = new();
    private readonly UpdateShipmentItemCommandHandler _handler;
    private readonly Guid _shipmentItemId = Guid.NewGuid();
    private readonly Guid _shipmentId = Guid.NewGuid();
    private readonly Guid _skuId = Guid.NewGuid();
    private readonly Guid _warehouseId = Guid.NewGuid();
    private readonly DateTime _sendingDate = new(2026, 3, 1);

    public UpdateShipmentItemCommandHandlerTests()
    {
        _handler = new UpdateShipmentItemCommandHandler(
            _shipmentRepository.Object, _unitOfWork.Object, _catalogRepository.Object,
            _deliveryRepository.Object, _stockStateRepository.Object);
    }

    private ShipmentItem ExistingItem() => new() { Id = _shipmentItemId, ShipmentId = _shipmentId, SkuId = Guid.NewGuid(), Quantity = 4 };
    private WHMS.Domain.Entities.Shipment ExistingShipment() => new()
    {
        Id = _shipmentId, WarehouseId = _warehouseId, ExpectedSendingDate = _sendingDate, SendingDate = null
    };

    private UpdateShipmentItemCommandRequest Request(int quantity) => new()
    {
        ShipmentItemId = _shipmentItemId.ToString(), ShipmentId = _shipmentId.ToString(), SkuId = _skuId.ToString(), Quantity = quantity
    };

    private void SetProjectedStock(int currentStock, int incomingForThisSku, int alreadyPlannedForThisSku)
    {
        _stockStateRepository.Setup(r => r.GetStockQuantity(_warehouseId, _skuId)).ReturnsAsync(currentStock);
        var deliveryItem = new DeliveryItem { Id = Guid.NewGuid(), SkuId = _skuId, Quantity = incomingForThisSku };
        var delivery = new WHMS.Domain.Entities.Delivery { Id = Guid.NewGuid(), DeliveryItems = [deliveryItem] };
        _deliveryRepository.Setup(r => r.GetDeliveries(It.IsAny<DeliveryFilter>(), false)).ReturnsAsync([delivery]);

        var plannedItem = new ShipmentItem { Id = Guid.NewGuid(), SkuId = _skuId, Quantity = alreadyPlannedForThisSku };
        var otherShipment = new WHMS.Domain.Entities.Shipment { Id = Guid.NewGuid(), ShipmentItems = [plannedItem] };
        _shipmentRepository.Setup(r => r.GetShipments(It.IsAny<ShipmentFilter>(), false)).ReturnsAsync([otherShipment]);
    }

    [Fact]
    public async Task Handle_ShipmentItemNotFound_ThrowsAndDoesNotCommit()
    {
        _shipmentRepository.Setup(r => r.GetShipmentItem(_shipmentItemId)).ReturnsAsync((ShipmentItem)null!);

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(Request(1), CancellationToken.None));

        Assert.Equal("Shipment item not found.", exception.Message);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_ParentShipmentNotFound_Throws()
    {
        _shipmentRepository.Setup(r => r.GetShipmentItem(_shipmentItemId)).ReturnsAsync(ExistingItem());
        _shipmentRepository.Setup(r => r.GetShipment(_shipmentId)).ReturnsAsync((WHMS.Domain.Entities.Shipment)null!);

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(Request(1), CancellationToken.None));

        Assert.Equal("Shipment not found.", exception.Message);
    }

    [Fact]
    public async Task Handle_ShipmentAlreadySent_ThrowsWithoutCheckingStock()
    {
        _shipmentRepository.Setup(r => r.GetShipmentItem(_shipmentItemId)).ReturnsAsync(ExistingItem());
        var shipment = ExistingShipment();
        shipment.SendingDate = DateTime.Now;
        _shipmentRepository.Setup(r => r.GetShipment(_shipmentId)).ReturnsAsync(shipment);

        var exception = await Assert.ThrowsAsync<BusinessRuleViolationException>(() => _handler.Handle(Request(1), CancellationToken.None));

        Assert.Equal("Shipment has alread been sent.", exception.Message);
        _stockStateRepository.Verify(r => r.GetStockQuantity(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ProjectedStockBelowRequestedQuantity_Throws()
    {
        _shipmentRepository.Setup(r => r.GetShipmentItem(_shipmentItemId)).ReturnsAsync(ExistingItem());
        _shipmentRepository.Setup(r => r.GetShipment(_shipmentId)).ReturnsAsync(ExistingShipment());
        SetProjectedStock(currentStock: 10, incomingForThisSku: 5, alreadyPlannedForThisSku: 3);

        var exception = await Assert.ThrowsAsync<BusinessRuleViolationException>(() => _handler.Handle(Request(13), CancellationToken.None));

        Assert.Equal("Insufficient stock in warehouse for the scheduled shipment date.", exception.Message);
        _catalogRepository.Verify(r => r.GetSku(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task Handle_NewSkuNotFound_ThrowsAndLeavesItemUnchanged()
    {
        var item = ExistingItem();
        var originalSkuId = item.SkuId;
        _shipmentRepository.Setup(r => r.GetShipmentItem(_shipmentItemId)).ReturnsAsync(item);
        _shipmentRepository.Setup(r => r.GetShipment(_shipmentId)).ReturnsAsync(ExistingShipment());
        SetProjectedStock(currentStock: 10, incomingForThisSku: 5, alreadyPlannedForThisSku: 3);
        _catalogRepository.Setup(r => r.GetSku(_skuId)).ReturnsAsync((SKU)null!);

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(Request(12), CancellationToken.None));

        // Message now sourced from the shared SkuRules.EnsureExists ("Sku not found.", not "SKU not found.").
        Assert.Equal("Sku not found.", exception.Message);
        Assert.Equal(originalSkuId, item.SkuId);
        Assert.Equal(4, item.Quantity);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_ValidUpdate_UpdatesItemAndReturnsResponse()
    {
        var item = ExistingItem();
        _shipmentRepository.Setup(r => r.GetShipmentItem(_shipmentItemId)).ReturnsAsync(item);
        _shipmentRepository.Setup(r => r.GetShipment(_shipmentId)).ReturnsAsync(ExistingShipment());
        SetProjectedStock(currentStock: 10, incomingForThisSku: 5, alreadyPlannedForThisSku: 3);
        var sku = new SKU { Id = _skuId, SKUName = "kola", NormalizedSKUName = "KOLA", Barcode = "8690000000001" };
        _catalogRepository.Setup(r => r.GetSku(_skuId)).ReturnsAsync(sku);

        var response = await _handler.Handle(Request(12), CancellationToken.None);

        Assert.Equal(_skuId, item.SkuId);
        Assert.Equal(12, item.Quantity);
        _shipmentRepository.Verify(r => r.UpdateShipmentItem(item), Times.Once);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        Assert.Equal("kola", response.SkuName);
        Assert.Equal(12, response.Quantity);
    }
}
