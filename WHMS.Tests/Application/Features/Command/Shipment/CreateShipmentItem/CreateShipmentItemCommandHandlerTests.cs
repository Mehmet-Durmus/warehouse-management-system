using Moq;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Common.Filtering.Filters;
using WHMS.Application.Features.Command.Shipment.CreateShipmentItem;
using WHMS.Domain.Entities;
using WHMS.Domain.Exceptions;

namespace WHMS.Tests.Application.Features.Command.Shipment.CreateShipmentItem;

public class CreateShipmentItemCommandHandlerTests
{
    private readonly Mock<IShipmentRepository> _shipmentRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<ICatalogRepository> _catalogRepository = new();
    private readonly Mock<IDeliveryRepository> _deliveryRepository = new();
    private readonly Mock<IStockStateRepository> _stockStateRepository = new();
    private readonly CreateShipmentItemCommandHandler _handler;
    private readonly Guid _shipmentId = Guid.NewGuid();
    private readonly Guid _skuId = Guid.NewGuid();
    private readonly Guid _warehouseId = Guid.NewGuid();
    private readonly DateTime _sendingDate = new(2026, 3, 1);

    public CreateShipmentItemCommandHandlerTests()
    {
        _handler = new CreateShipmentItemCommandHandler(
            _shipmentRepository.Object, _unitOfWork.Object, _catalogRepository.Object,
            _deliveryRepository.Object, _stockStateRepository.Object);
    }

    private WHMS.Domain.Entities.Shipment ExistingShipment() => new()
    {
        Id = _shipmentId, WarehouseId = _warehouseId, ExpectedSendingDate = _sendingDate, SendingDate = null
    };

    private CreateShipmentItemCommandRequest Request(int quantity) => new()
    {
        ShipmentId = _shipmentId.ToString(), SkuId = _skuId.ToString(), Quantity = quantity
    };

    private void SetProjectedStock(int currentStock, int incomingForThisSku, int alreadyPlannedForThisSku)
    {
        _stockStateRepository.Setup(r => r.GetStockQuantity(_warehouseId, _skuId)).ReturnsAsync(currentStock);

        var matchingDeliveryItem = new DeliveryItem { Id = Guid.NewGuid(), SkuId = _skuId, Quantity = incomingForThisSku };
        var otherSkuDeliveryItem = new DeliveryItem { Id = Guid.NewGuid(), SkuId = Guid.NewGuid(), Quantity = 999 };
        var delivery = new WHMS.Domain.Entities.Delivery { Id = Guid.NewGuid(), DeliveryItems = [matchingDeliveryItem, otherSkuDeliveryItem] };
        _deliveryRepository
            .Setup(r => r.GetDeliveries(It.Is<DeliveryFilter>(f => f.WarehouseId == _warehouseId.ToString() && f.ExpectedArrivalBefore == _sendingDate), false))
            .ReturnsAsync([delivery]);

        var matchingShipmentItem = new ShipmentItem { Id = Guid.NewGuid(), SkuId = _skuId, Quantity = alreadyPlannedForThisSku };
        var otherSkuShipmentItem = new ShipmentItem { Id = Guid.NewGuid(), SkuId = Guid.NewGuid(), Quantity = 999 };
        var otherShipment = new WHMS.Domain.Entities.Shipment { Id = Guid.NewGuid(), ShipmentItems = [matchingShipmentItem, otherSkuShipmentItem] };
        _shipmentRepository
            .Setup(r => r.GetShipments(It.Is<ShipmentFilter>(f => f.WarehouseId == _warehouseId.ToString() && f.ExpectedSendingDateBefore == _sendingDate), false))
            .ReturnsAsync([otherShipment]);
    }

    [Fact]
    public async Task Handle_ShipmentNotFound_ThrowsAndDoesNotAdd()
    {
        _shipmentRepository.Setup(r => r.GetShipment(_shipmentId)).ReturnsAsync((WHMS.Domain.Entities.Shipment)null!);

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(Request(1), CancellationToken.None));

        Assert.Equal("Shipment not found.", exception.Message);
        _shipmentRepository.Verify(r => r.AddShipmentItem(It.IsAny<ShipmentItem>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShipmentAlreadySent_ThrowsAndDoesNotAdd()
    {
        var shipment = ExistingShipment();
        shipment.SendingDate = DateTime.Now;
        _shipmentRepository.Setup(r => r.GetShipment(_shipmentId)).ReturnsAsync(shipment);

        // Note: "alread" is a typo already present in the source; preserved as-is.
        var exception = await Assert.ThrowsAsync<BusinessRuleViolationException>(() => _handler.Handle(Request(1), CancellationToken.None));

        Assert.Equal("Shipment has alread been sent.", exception.Message);
        _shipmentRepository.Verify(r => r.AddShipmentItem(It.IsAny<ShipmentItem>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ProjectedStockBelowRequestedQuantity_ThrowsWithoutCheckingSku()
    {
        _shipmentRepository.Setup(r => r.GetShipment(_shipmentId)).ReturnsAsync(ExistingShipment());
        SetProjectedStock(currentStock: 10, incomingForThisSku: 5, alreadyPlannedForThisSku: 3);
        // expected available = 10 + 5 - 3 = 12; requesting 13 should fail.

        var exception = await Assert.ThrowsAsync<BusinessRuleViolationException>(() => _handler.Handle(Request(13), CancellationToken.None));

        Assert.Equal("Insufficient stock in warehouse for the scheduled shipment date.", exception.Message);
        _catalogRepository.Verify(r => r.GetSku(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ProjectedStockCoversRequestButSkuMissing_Throws()
    {
        _shipmentRepository.Setup(r => r.GetShipment(_shipmentId)).ReturnsAsync(ExistingShipment());
        SetProjectedStock(currentStock: 10, incomingForThisSku: 5, alreadyPlannedForThisSku: 3);
        _catalogRepository.Setup(r => r.GetSku(_skuId)).ReturnsAsync((SKU)null!);

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(Request(12), CancellationToken.None));

        // Message now sourced from the shared SkuRules.EnsureExists ("Sku not found.", not "SKU not found.").
        Assert.Equal("Sku not found.", exception.Message);
        _shipmentRepository.Verify(r => r.AddShipmentItem(It.IsAny<ShipmentItem>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ValidRequest_AddsItemAndReturnsResponse()
    {
        _shipmentRepository.Setup(r => r.GetShipment(_shipmentId)).ReturnsAsync(ExistingShipment());
        SetProjectedStock(currentStock: 10, incomingForThisSku: 5, alreadyPlannedForThisSku: 3);
        var sku = new SKU { Id = _skuId, SKUName = "kola", NormalizedSKUName = "KOLA", Barcode = "8690000000001" };
        _catalogRepository.Setup(r => r.GetSku(_skuId)).ReturnsAsync(sku);

        var response = await _handler.Handle(Request(12), CancellationToken.None);

        _shipmentRepository.Verify(r => r.AddShipmentItem(It.Is<ShipmentItem>(i =>
            i.ShipmentId == _shipmentId && i.SkuId == _skuId && i.Quantity == 12)), Times.Once);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        Assert.Equal("kola", response.SkuName);
        Assert.Equal(12, response.Quantity);
    }
}
