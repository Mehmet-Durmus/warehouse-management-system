using Moq;
using WHMS.Application.Abstractions.Infrastructure;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Common.Constants;
using WHMS.Application.Features.Queries.Shipment.GetShipment;
using WHMS.Domain.ValueObjects;

namespace WHMS.Tests.Application.Features.Queries.Shipment.GetShipment;

public class GetShipmentQueryHandlerTests
{
    private readonly Mock<IShipmentRepository> _shipmentRepository = new();
    private readonly Mock<ICurrentUserService> _currentUserService = new();
    private readonly GetShipmentQueryHandler _handler;
    private readonly Guid _shipmentId = Guid.NewGuid();
    private readonly Guid _warehouseId = Guid.NewGuid();
    private readonly Guid _otherWarehouseId = Guid.NewGuid();

    public GetShipmentQueryHandlerTests()
    {
        _handler = new GetShipmentQueryHandler(_shipmentRepository.Object, _currentUserService.Object);
    }

    private void SetCurrentUser(string role) => _currentUserService.Setup(u => u.Roles).Returns([role]);
    private GetShipmentQueryRequest Request() => new() { ShipmentId = _shipmentId.ToString() };

    private static Address MakeAddress() => new(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "34000", "Cadde");

    private WHMS.Domain.Entities.Shipment MakeShipment(Guid warehouseId, bool sent) => new()
    {
        Id = _shipmentId,
        WarehouseId = warehouseId,
        SendingDate = sent ? DateTime.Now : null,
        Warehouse = new WHMS.Domain.Entities.Warehouse { WarehouseName = "Depo", Address = MakeAddress() },
        Store = new WHMS.Domain.Entities.Store { StoreName = "Magaza", NormalizedName = "MAGAZA", Address = MakeAddress() }
    };

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
        var shipment = MakeShipment(sameWarehouse ? _warehouseId : _otherWarehouseId, sent);
        _shipmentRepository.Setup(r => r.GetShipment(_shipmentId)).ReturnsAsync(shipment);

        var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(Request(), CancellationToken.None));

        Assert.Equal("Shipment not found.", exception.Message);
    }

    [Fact]
    public async Task Handle_StaffSameWarehouseNotYetSent_ReturnsResponse()
    {
        SetCurrentUser(ApplicationRole.WarehouseStaff);
        _currentUserService.Setup(u => u.WarehouseId).Returns(_warehouseId.ToString());
        var shipment = MakeShipment(_warehouseId, sent: false);
        _shipmentRepository.Setup(r => r.GetShipment(_shipmentId)).ReturnsAsync(shipment);

        var response = await _handler.Handle(Request(), CancellationToken.None);

        Assert.Equal("Depo", response.WarehouseName);
        Assert.Equal("Magaza", response.StoreName);
        Assert.Null(response.SendingDate);
    }

    [Fact]
    public async Task Handle_ManagerSameWarehouseAlreadySent_StillReturnsResponse()
    {
        SetCurrentUser(ApplicationRole.WarehouseManager);
        _currentUserService.Setup(u => u.WarehouseId).Returns(_warehouseId.ToString());
        var shipment = MakeShipment(_warehouseId, sent: true);
        _shipmentRepository.Setup(r => r.GetShipment(_shipmentId)).ReturnsAsync(shipment);

        var response = await _handler.Handle(Request(), CancellationToken.None);

        Assert.NotNull(response.SendingDate);
    }
}
