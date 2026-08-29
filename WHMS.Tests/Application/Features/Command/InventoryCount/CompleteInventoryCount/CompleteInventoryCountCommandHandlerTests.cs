using Moq;
using WHMS.Application.Abstractions.Infrastructure;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Features.Command.InventoryCount.CompleteInventoryCount;
using WHMS.Domain.Entities;
using WHMS.Domain.Exceptions;

namespace WHMS.Tests.Application.Features.Command.InventoryCount.CompleteInventoryCount;

public class CompleteInventoryCountCommandHandlerTests
{
    private readonly Mock<IInventoryCountRepository> _inventoryCountRepository = new();
    private readonly Mock<IStockStateRepository> _stockStateRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<ICurrentUserService> _currentUserService = new();
    private readonly CompleteInventoryCountCommandHandler _handler;
    private readonly Guid _warehouseId = Guid.NewGuid();
    private readonly Guid _inventoryCountId = Guid.NewGuid();

    public CompleteInventoryCountCommandHandlerTests()
    {
        _currentUserService.Setup(u => u.WarehouseId).Returns(_warehouseId.ToString());
        _handler = new CompleteInventoryCountCommandHandler(
            _inventoryCountRepository.Object, _stockStateRepository.Object, _unitOfWork.Object, _currentUserService.Object);
    }

    private CompleteInventoryCountCommandRequest Request() => new() { InventoryCountId = _inventoryCountId.ToString() };

    [Fact]
    public async Task Handle_NotFound_ThrowsAndDoesNotCommit()
    {
        _inventoryCountRepository.Setup(r => r.GetInventoryCount(_inventoryCountId)).ReturnsAsync((WHMS.Domain.Entities.InventoryCount)null!);

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(Request(), CancellationToken.None));

        Assert.Equal("Inventory count not found.", exception.Message);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_BelongsToAnotherWarehouse_ThrowsSameNotFoundMessage()
    {
        var inventoryCount = new WHMS.Domain.Entities.InventoryCount { Id = _inventoryCountId, WarehouseId = Guid.NewGuid() };
        _inventoryCountRepository.Setup(r => r.GetInventoryCount(_inventoryCountId)).ReturnsAsync(inventoryCount);

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(Request(), CancellationToken.None));

        Assert.Equal("Inventory count not found.", exception.Message);
    }

    [Fact]
    public async Task Handle_HasLines_SetsStockToCountedQuantityForEachLineAndCompletes()
    {
        var line1 = new InventoryCountLine { Id = Guid.NewGuid(), SkuId = Guid.NewGuid(), Quantity = 10 };
        var line2 = new InventoryCountLine { Id = Guid.NewGuid(), SkuId = Guid.NewGuid(), Quantity = 4 };
        var inventoryCount = new WHMS.Domain.Entities.InventoryCount
        {
            Id = _inventoryCountId, WarehouseId = _warehouseId, IsCompleted = false, InventoryCountLines = [line1, line2]
        };
        _inventoryCountRepository.Setup(r => r.GetInventoryCount(_inventoryCountId)).ReturnsAsync(inventoryCount);

        await _handler.Handle(Request(), CancellationToken.None);

        Assert.True(inventoryCount.IsCompleted);
        _stockStateRepository.Verify(r => r.SetQuantity(_warehouseId, line1.SkuId, 10), Times.Once);
        _stockStateRepository.Verify(r => r.SetQuantity(_warehouseId, line2.SkuId, 4), Times.Once);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_NoLines_CompletesWithoutTouchingStock()
    {
        var inventoryCount = new WHMS.Domain.Entities.InventoryCount
        {
            Id = _inventoryCountId, WarehouseId = _warehouseId, IsCompleted = false, InventoryCountLines = null
        };
        _inventoryCountRepository.Setup(r => r.GetInventoryCount(_inventoryCountId)).ReturnsAsync(inventoryCount);

        await _handler.Handle(Request(), CancellationToken.None);

        Assert.True(inventoryCount.IsCompleted);
        _stockStateRepository.Verify(r => r.SetQuantity(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task Handle_AlreadyCompleted_DoesNotThrowAndReappliesStockQuantities()
    {
        // Documents a current gap, not intended behavior: unlike its sibling
        // InventoryCount handlers, this one has no IsCompleted guard, so completing
        // an already-completed count silently succeeds again. Confirmed with the
        // project owner as low-impact (sibling guards already prevent line changes
        // once completed, so this just redundantly re-applies the same values) and
        // left as-is for now.
        var line = new InventoryCountLine { Id = Guid.NewGuid(), SkuId = Guid.NewGuid(), Quantity = 7 };
        var inventoryCount = new WHMS.Domain.Entities.InventoryCount
        {
            Id = _inventoryCountId, WarehouseId = _warehouseId, IsCompleted = true, InventoryCountLines = [line]
        };
        _inventoryCountRepository.Setup(r => r.GetInventoryCount(_inventoryCountId)).ReturnsAsync(inventoryCount);

        await _handler.Handle(Request(), CancellationToken.None);

        _stockStateRepository.Verify(r => r.SetQuantity(_warehouseId, line.SkuId, 7), Times.Once);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
    }
}
