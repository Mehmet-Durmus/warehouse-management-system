using Moq;
using WHMS.Application.Abstractions.Infrastructure;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Common.Constants;
using WHMS.Application.Common.Filtering.Filters;
using WHMS.Application.Features.Queries.InventoryCount.GetInventoryCountLines;
using WHMS.Domain.Entities;

namespace WHMS.Tests.Application.Features.Queries.InventoryCount.GetInventoryCountLines;

public class GetInventoryCountLinesQueryHandlerTests
{
    private readonly Mock<IInventoryCountRepository> _inventoryCountRepository = new();
    private readonly Mock<ICurrentUserService> _currentUserService = new();
    private readonly GetInventoryCountLinesQueryHandler _handler;
    private readonly Guid _inventoryCountId = Guid.NewGuid();
    private readonly Guid _warehouseId = Guid.NewGuid();
    private readonly Guid _otherWarehouseId = Guid.NewGuid();

    public GetInventoryCountLinesQueryHandlerTests()
    {
        _handler = new GetInventoryCountLinesQueryHandler(_inventoryCountRepository.Object, _currentUserService.Object);
    }

    private void SetCurrentUser(string role) => _currentUserService.Setup(u => u.Roles).Returns([role]);
    private GetInventoryCountLinesQueryRequest Request() => new() { InventoryCountId = _inventoryCountId.ToString(), Page = 1, PageSize = 20 };

    [Fact]
    public async Task Handle_NotFound_Throws()
    {
        SetCurrentUser(ApplicationRole.LogisticDirector);
        _inventoryCountRepository.Setup(r => r.GetInventoryCount(_inventoryCountId)).ReturnsAsync((WHMS.Domain.Entities.InventoryCount)null!);

        var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(Request(), CancellationToken.None));

        Assert.Equal("Inventory count not found.", exception.Message);
    }

    [Theory]
    [InlineData(ApplicationRole.WarehouseManager, false, true)]
    [InlineData(ApplicationRole.WarehouseStaff, false, true)]
    [InlineData(ApplicationRole.WarehouseStaff, true, false)]
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
    public async Task Handle_StaffSameWarehouseCompleted_ReturnsPaginatedLines()
    {
        SetCurrentUser(ApplicationRole.WarehouseStaff);
        _currentUserService.Setup(u => u.WarehouseId).Returns(_warehouseId.ToString());
        var inventoryCount = new WHMS.Domain.Entities.InventoryCount { Id = _inventoryCountId, WarehouseId = _warehouseId, IsCompleted = true };
        _inventoryCountRepository.Setup(r => r.GetInventoryCount(_inventoryCountId)).ReturnsAsync(inventoryCount);
        var line = new InventoryCountLine
        {
            Id = Guid.NewGuid(), Quantity = 5, Variance = -1,
            Sku = new SKU { Id = Guid.NewGuid(), SKUName = "kola", NormalizedSKUName = "KOLA", Barcode = "8690000000001" }
        };
        _inventoryCountRepository
            .Setup(r => r.GetInventoryCountLines(It.Is<InventoryCountLineFilter>(f => f.InventoryCountId == _inventoryCountId.ToString()), true))
            .ReturnsAsync([line]);
        _inventoryCountRepository.Setup(r => r.CountInventoryCountLines(It.IsAny<InventoryCountLineFilter>())).ReturnsAsync(1);

        var response = await _handler.Handle(Request(), CancellationToken.None);

        Assert.Single(response.InventoryCountLines);
        Assert.Equal("kola", response.InventoryCountLines[0].SkuName);
    }
}
