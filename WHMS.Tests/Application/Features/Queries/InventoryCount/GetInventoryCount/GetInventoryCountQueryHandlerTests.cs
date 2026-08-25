using Moq;
using WHMS.Application.Abstractions.Infrastructure;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Common.Constants;
using WHMS.Application.Features.Queries.InventoryCount.GetInventoryCount;

namespace WHMS.Tests.Application.Features.Queries.InventoryCount.GetInventoryCount;

public class GetInventoryCountQueryHandlerTests
{
    private readonly Mock<IInventoryCountRepository> _inventoryCountRepository = new();
    private readonly Mock<ICurrentUserService> _currentUserService = new();
    private readonly GetInventoryCountQueryHandler _handler;
    private readonly Guid _inventoryCountId = Guid.NewGuid();
    private readonly Guid _warehouseId = Guid.NewGuid();
    private readonly Guid _otherWarehouseId = Guid.NewGuid();

    public GetInventoryCountQueryHandlerTests()
    {
        _handler = new GetInventoryCountQueryHandler(_inventoryCountRepository.Object, _currentUserService.Object);
    }

    private void SetCurrentUser(string role) => _currentUserService.Setup(u => u.Roles).Returns([role]);
    private GetInventoryCountQueryRequest Request() => new() { InventoryCountId = _inventoryCountId.ToString() };

    [Fact]
    public async Task Handle_NotFound_Throws()
    {
        SetCurrentUser(ApplicationRole.LogisticDirector);
        _inventoryCountRepository.Setup(r => r.GetInventoryCount(_inventoryCountId)).ReturnsAsync((WHMS.Domain.Entities.InventoryCount)null!);

        var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(Request(), CancellationToken.None));

        Assert.Equal("Inventory count not found.", exception.Message);
    }

    [Theory]
    [InlineData(ApplicationRole.WarehouseManager, false, true)]  // manager, different warehouse
    [InlineData(ApplicationRole.WarehouseStaff, false, true)]    // staff, different warehouse
    [InlineData(ApplicationRole.WarehouseStaff, true, false)]    // staff, same warehouse, NOT yet completed
    public async Task Handle_AccessDenied_ThrowsNotFound(string role, bool sameWarehouse, bool completed)
    {
        SetCurrentUser(role);
        _currentUserService.Setup(u => u.WarehouseId).Returns(_warehouseId.ToString());
        var inventoryCount = new WHMS.Domain.Entities.InventoryCount
        {
            Id = _inventoryCountId,
            WarehouseId = sameWarehouse ? _warehouseId : _otherWarehouseId,
            IsCompleted = completed
        };
        _inventoryCountRepository.Setup(r => r.GetInventoryCount(_inventoryCountId)).ReturnsAsync(inventoryCount);

        var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(Request(), CancellationToken.None));

        Assert.Equal("Inventory count not found.", exception.Message);
    }

    [Fact]
    public async Task Handle_StaffSameWarehouseCompleted_ReturnsResponse()
    {
        // Staff can only view completed counts - the opposite of the
        // Delivery/Shipment pattern, where staff is blocked once the entity is
        // received/sent rather than while it's still in progress.
        SetCurrentUser(ApplicationRole.WarehouseStaff);
        _currentUserService.Setup(u => u.WarehouseId).Returns(_warehouseId.ToString());
        var inventoryCount = new WHMS.Domain.Entities.InventoryCount { Id = _inventoryCountId, WarehouseId = _warehouseId, IsCompleted = true };
        _inventoryCountRepository.Setup(r => r.GetInventoryCount(_inventoryCountId)).ReturnsAsync(inventoryCount);

        var response = await _handler.Handle(Request(), CancellationToken.None);

        Assert.True(response.IsCompleted);
    }

    [Fact]
    public async Task Handle_ManagerSameWarehouseNotCompleted_StillReturnsResponse()
    {
        SetCurrentUser(ApplicationRole.WarehouseManager);
        _currentUserService.Setup(u => u.WarehouseId).Returns(_warehouseId.ToString());
        var inventoryCount = new WHMS.Domain.Entities.InventoryCount { Id = _inventoryCountId, WarehouseId = _warehouseId, IsCompleted = false };
        _inventoryCountRepository.Setup(r => r.GetInventoryCount(_inventoryCountId)).ReturnsAsync(inventoryCount);

        var response = await _handler.Handle(Request(), CancellationToken.None);

        Assert.False(response.IsCompleted);
    }
}
