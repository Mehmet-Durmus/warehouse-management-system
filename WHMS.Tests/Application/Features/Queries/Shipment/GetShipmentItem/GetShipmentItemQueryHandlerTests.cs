using Moq;
using WHMS.Application.Abstractions.Infrastructure;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Common.Constants;
using WHMS.Application.Features.Queries.Shipment.GetShipmentItem;
using WHMS.Domain.Entities;

namespace WHMS.Tests.Application.Features.Queries.Shipment.GetShipmentItem;

public class GetShipmentItemQueryHandlerTests
{
    private readonly Mock<IShipmentRepository> _shipmentRepository = new();
    private readonly Mock<ICurrentUserService> _currentUserService = new();
    private readonly GetShipmentItemQueryHandler _handler;
    private readonly Guid _shipmentItemId = Guid.NewGuid();
    private readonly Guid _warehouseId = Guid.NewGuid();
    private readonly Guid _otherWarehouseId = Guid.NewGuid();

    public GetShipmentItemQueryHandlerTests()
    {
        _handler = new GetShipmentItemQueryHandler(_shipmentRepository.Object, _currentUserService.Object);
    }

    private void SetCurrentUser(string role) => _currentUserService.Setup(u => u.Roles).Returns([role]);
    private GetShipmentItemQueryRequest Request() => new() { ShipmentItemId = _shipmentItemId.ToString() };

    private ShipmentItem MakeItem(Guid warehouseId, bool sent) => new()
    {
        Id = _shipmentItemId,
        SkuId = Guid.NewGuid(),
        Quantity = 6,
        SKU = new SKU { Id = Guid.NewGuid(), SKUName = "kola", NormalizedSKUName = "KOLA", Barcode = "8690000000001" },
        Shipment = new WHMS.Domain.Entities.Shipment { WarehouseId = warehouseId, SendingDate = sent ? DateTime.Now : null }
    };

    [Fact]
    public async Task Handle_ItemNotFound_Throws()
    {
        SetCurrentUser(ApplicationRole.LogisticDirector);
        _shipmentRepository.Setup(r => r.GetShipmentItem(_shipmentItemId)).ReturnsAsync((ShipmentItem)null!);

        var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(Request(), CancellationToken.None));

        Assert.Equal("Shipment item not found.", exception.Message);
    }

    [Theory]
    [InlineData(ApplicationRole.WarehouseManager, false, false)]
    [InlineData(ApplicationRole.WarehouseStaff, false, false)]
    [InlineData(ApplicationRole.WarehouseStaff, true, true)]
    public async Task Handle_AccessDenied_ThrowsNotFound(string role, bool sameWarehouse, bool sent)
    {
        SetCurrentUser(role);
        _currentUserService.Setup(u => u.WarehouseId).Returns(_warehouseId.ToString());
        var item = MakeItem(sameWarehouse ? _warehouseId : _otherWarehouseId, sent);
        _shipmentRepository.Setup(r => r.GetShipmentItem(_shipmentItemId)).ReturnsAsync(item);

        var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(Request(), CancellationToken.None));

        Assert.Equal("Shipment item not found.", exception.Message);
    }

    [Fact]
    public async Task Handle_StaffSameWarehouseNotYetSent_ReturnsResponse()
    {
        SetCurrentUser(ApplicationRole.WarehouseStaff);
        _currentUserService.Setup(u => u.WarehouseId).Returns(_warehouseId.ToString());
        var item = MakeItem(_warehouseId, sent: false);
        _shipmentRepository.Setup(r => r.GetShipmentItem(_shipmentItemId)).ReturnsAsync(item);

        var response = await _handler.Handle(Request(), CancellationToken.None);

        Assert.Equal("kola", response.Sku.SkuName);
        Assert.Equal(6, response.Quantity);
    }
}
