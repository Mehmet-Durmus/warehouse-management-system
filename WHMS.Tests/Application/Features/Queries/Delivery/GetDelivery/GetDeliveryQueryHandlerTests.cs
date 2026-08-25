using Moq;
using WHMS.Application.Abstractions.Infrastructure;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Common.Constants;
using WHMS.Application.Features.Queries.Delivery.GetDelivery;
using WHMS.Domain.Entities;

namespace WHMS.Tests.Application.Features.Queries.Delivery.GetDelivery;

public class GetDeliveryQueryHandlerTests
{
    private readonly Mock<IDeliveryRepository> _deliveryRepository = new();
    private readonly Mock<ICurrentUserService> _currentUserService = new();
    private readonly GetDeliveryQueryHandler _handler;
    private readonly Guid _deliveryId = Guid.NewGuid();
    private readonly Guid _warehouseId = Guid.NewGuid();
    private readonly Guid _otherWarehouseId = Guid.NewGuid();

    public GetDeliveryQueryHandlerTests()
    {
        _handler = new GetDeliveryQueryHandler(_deliveryRepository.Object, _currentUserService.Object);
    }

    private void SetCurrentUser(string role) => _currentUserService.Setup(u => u.Roles).Returns([role]);
    private GetDeliveryQueryRequet Request() => new() { DeliveryId = _deliveryId.ToString() };
    private static WHMS.Domain.Entities.Warehouse MakeWarehouse() => new()
    {
        WarehouseName = "Depo",
        Address = new WHMS.Domain.ValueObjects.Address(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "34000", "Cadde")
    };

    [Fact]
    public async Task Handle_DeliveryNotFound_Throws()
    {
        SetCurrentUser(ApplicationRole.LogisticDirector);
        _deliveryRepository.Setup(r => r.GetDelivery(_deliveryId)).ReturnsAsync((WHMS.Domain.Entities.Delivery)null!);

        var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(Request(), CancellationToken.None));

        Assert.Equal("Delivery not found.", exception.Message);
    }

    [Theory]
    [InlineData(ApplicationRole.WarehouseManager, false, false)]  // manager, different warehouse
    [InlineData(ApplicationRole.WarehouseStaff, false, false)]    // staff, different warehouse
    [InlineData(ApplicationRole.WarehouseStaff, true, true)]      // staff, same warehouse, already received
    public async Task Handle_AccessDenied_ThrowsNotFound(string role, bool sameWarehouse, bool received)
    {
        SetCurrentUser(role);
        _currentUserService.Setup(u => u.WarehouseId).Returns(_warehouseId.ToString());
        var delivery = new WHMS.Domain.Entities.Delivery
        {
            Id = _deliveryId,
            WarehouseId = sameWarehouse ? _warehouseId : _otherWarehouseId,
            ReceivedAt = received ? DateTime.Now : null,
            Warehouse = MakeWarehouse()
        };
        _deliveryRepository.Setup(r => r.GetDelivery(_deliveryId)).ReturnsAsync(delivery);

        var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(Request(), CancellationToken.None));

        Assert.Equal("Delivery not found.", exception.Message);
    }

    [Fact]
    public async Task Handle_StaffSameWarehouseNotYetReceived_ReturnsResponse()
    {
        SetCurrentUser(ApplicationRole.WarehouseStaff);
        _currentUserService.Setup(u => u.WarehouseId).Returns(_warehouseId.ToString());
        var delivery = new WHMS.Domain.Entities.Delivery
        {
            Id = _deliveryId, WarehouseId = _warehouseId, ReceivedAt = null,
            Warehouse = MakeWarehouse()
        };
        _deliveryRepository.Setup(r => r.GetDelivery(_deliveryId)).ReturnsAsync(delivery);

        var response = await _handler.Handle(Request(), CancellationToken.None);

        Assert.False(response.IsReceived);
        Assert.Equal("Depo", response.WarehouseName);
    }

    [Fact]
    public async Task Handle_ManagerSameWarehouseAlreadyReceived_StillReturnsResponse()
    {
        // Unlike staff, the manager branch has no "already received" restriction.
        SetCurrentUser(ApplicationRole.WarehouseManager);
        _currentUserService.Setup(u => u.WarehouseId).Returns(_warehouseId.ToString());
        var delivery = new WHMS.Domain.Entities.Delivery
        {
            Id = _deliveryId, WarehouseId = _warehouseId, ReceivedAt = DateTime.Now,
            Warehouse = MakeWarehouse()
        };
        _deliveryRepository.Setup(r => r.GetDelivery(_deliveryId)).ReturnsAsync(delivery);

        var response = await _handler.Handle(Request(), CancellationToken.None);

        Assert.True(response.IsReceived);
    }

    [Fact]
    public async Task Handle_LogisticDirector_BypassesWarehouseScoping()
    {
        SetCurrentUser(ApplicationRole.LogisticDirector);
        var delivery = new WHMS.Domain.Entities.Delivery
        {
            Id = _deliveryId, WarehouseId = _otherWarehouseId, ReceivedAt = null,
            Warehouse = MakeWarehouse()
        };
        _deliveryRepository.Setup(r => r.GetDelivery(_deliveryId)).ReturnsAsync(delivery);

        var response = await _handler.Handle(Request(), CancellationToken.None);

        Assert.Equal(_otherWarehouseId.ToString(), response.WarehouseId);
    }
}
