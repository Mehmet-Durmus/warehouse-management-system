using Moq;
using WHMS.Application.Abstractions.Infrastructure;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Common.Constants;
using WHMS.Application.Common.Filtering.Filters;
using WHMS.Application.Features.Queries.Shipment.GetShipmentItems;
using WHMS.Domain.Entities;

namespace WHMS.Tests.Application.Features.Queries.Shipment.GetShipmentItems;

public class GetShipmentItemsQueryHandlerTests
{
    private readonly Mock<IShipmentRepository> _shipmentRepository = new();
    private readonly Mock<ICurrentUserService> _currentUserService = new();
    private readonly GetShipmentItemsQueryHandler _handler;
    private readonly Guid _shipmentId = Guid.NewGuid();
    private readonly Guid _warehouseId = Guid.NewGuid();
    private readonly Guid _otherWarehouseId = Guid.NewGuid();

    public GetShipmentItemsQueryHandlerTests()
    {
        _handler = new GetShipmentItemsQueryHandler(_shipmentRepository.Object, _currentUserService.Object);
    }

    private void SetCurrentUser(string role) => _currentUserService.Setup(u => u.Roles).Returns([role]);
    private GetShipmentItemsQueryRequest Request() => new() { ShipmentId = _shipmentId.ToString(), Page = 1, PageSize = 20 };

    [Fact]
    public async Task Handle_ShipmentNotFound_Throws()
    {
        SetCurrentUser(ApplicationRole.LogisticDirector);
        _shipmentRepository.Setup(r => r.GetShipment(_shipmentId)).ReturnsAsync((WHMS.Domain.Entities.Shipment)null!);

        var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(Request(), CancellationToken.None));

        Assert.Equal("Shipment not found.", exception.Message);
    }

    [Theory]
    [InlineData(ApplicationRole.WarehouseManager, false, false)]
    [InlineData(ApplicationRole.WarehouseStaff, false, false)]
    [InlineData(ApplicationRole.WarehouseStaff, true, true)]
    public async Task Handle_AccessDenied_ThrowsNotFound(string role, bool sameWarehouse, bool sent)
    {
        SetCurrentUser(role);
        _currentUserService.Setup(u => u.WarehouseId).Returns(_warehouseId.ToString());
        var shipment = new WHMS.Domain.Entities.Shipment
        {
            Id = _shipmentId,
            WarehouseId = sameWarehouse ? _warehouseId : _otherWarehouseId,
            SendingDate = sent ? DateTime.Now : null
        };
        _shipmentRepository.Setup(r => r.GetShipment(_shipmentId)).ReturnsAsync(shipment);

        var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(Request(), CancellationToken.None));

        Assert.Equal("Shipment not found.", exception.Message);
    }

    [Fact]
    public async Task Handle_StaffSameWarehouseNotYetSent_ReturnsPaginatedItems()
    {
        SetCurrentUser(ApplicationRole.WarehouseStaff);
        _currentUserService.Setup(u => u.WarehouseId).Returns(_warehouseId.ToString());
        var shipment = new WHMS.Domain.Entities.Shipment { Id = _shipmentId, WarehouseId = _warehouseId, SendingDate = null };
        _shipmentRepository.Setup(r => r.GetShipment(_shipmentId)).ReturnsAsync(shipment);
        var item = new ShipmentItem
        {
            Id = Guid.NewGuid(), Quantity = 2,
            SKU = new SKU { Id = Guid.NewGuid(), SKUName = "kola", NormalizedSKUName = "KOLA", Barcode = "8690000000001" }
        };
        _shipmentRepository
            .Setup(r => r.GetShipmentItems(It.Is<ShipmentItemFilter>(f => f.ShipmentId == _shipmentId.ToString()), true))
            .ReturnsAsync([item]);
        _shipmentRepository.Setup(r => r.GetShipmentItemsCount(It.IsAny<ShipmentItemFilter>())).ReturnsAsync(1);

        var response = await _handler.Handle(Request(), CancellationToken.None);

        Assert.Single(response.ShipmentItems);
        Assert.Equal("kola", response.ShipmentItems[0].Sku.SKUName);
    }
}
