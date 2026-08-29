using Moq;
using WHMS.Application.Abstractions.Infrastructure;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Features.Command.InventoryCount.DeleteInventoryCount;
using WHMS.Domain.Entities;
using WHMS.Domain.Exceptions;

namespace WHMS.Tests.Application.Features.Command.InventoryCount.DeleteInventoryCount;

public class DeleteInventoryCountCommandHandlerTests
{
    private readonly Mock<IInventoryCountRepository> _inventoryCountRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<ICurrentUserService> _currentUserService = new();
    private readonly DeleteInventoryCountCommandHandler _handler;
    private readonly Guid _warehouseId = Guid.NewGuid();
    private readonly Guid _inventoryCountId = Guid.NewGuid();

    public DeleteInventoryCountCommandHandlerTests()
    {
        _currentUserService.Setup(u => u.WarehouseId).Returns(_warehouseId.ToString());
        _handler = new DeleteInventoryCountCommandHandler(_inventoryCountRepository.Object, _unitOfWork.Object, _currentUserService.Object);
    }

    private DeleteInventoryCountCommandRequest Request() => new() { InventoryCountId = _inventoryCountId.ToString() };

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
    public async Task Handle_AlreadyCompleted_ThrowsAndDoesNotDelete()
    {
        var inventoryCount = new WHMS.Domain.Entities.InventoryCount { Id = _inventoryCountId, WarehouseId = _warehouseId, IsCompleted = true };
        _inventoryCountRepository.Setup(r => r.GetInventoryCount(_inventoryCountId)).ReturnsAsync(inventoryCount);

        var exception = await Assert.ThrowsAsync<BusinessRuleViolationException>(() => _handler.Handle(Request(), CancellationToken.None));

        Assert.Equal("Inventory count has already been completed.", exception.Message);
        _inventoryCountRepository.Verify(r => r.DeleteInventoryCount(It.IsAny<WHMS.Domain.Entities.InventoryCount>()), Times.Never);
    }

    [Fact]
    public async Task Handle_HasLines_DeletesEveryLineAndTheInventoryCount()
    {
        var line1 = new InventoryCountLine { Id = Guid.NewGuid(), InventoryCountId = _inventoryCountId, SkuId = Guid.NewGuid() };
        var line2 = new InventoryCountLine { Id = Guid.NewGuid(), InventoryCountId = _inventoryCountId, SkuId = Guid.NewGuid() };
        var inventoryCount = new WHMS.Domain.Entities.InventoryCount
        {
            Id = _inventoryCountId, WarehouseId = _warehouseId, IsCompleted = false, InventoryCountLines = [line1, line2]
        };
        _inventoryCountRepository.Setup(r => r.GetInventoryCount(_inventoryCountId)).ReturnsAsync(inventoryCount);

        await _handler.Handle(Request(), CancellationToken.None);

        _inventoryCountRepository.Verify(r => r.DeleteInventoryCount(inventoryCount), Times.Once);
        _inventoryCountRepository.Verify(r => r.DeleteInventoryCountLine(line1), Times.Once);
        _inventoryCountRepository.Verify(r => r.DeleteInventoryCountLine(line2), Times.Once);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_NoLines_DeletesInventoryCountWithoutIteratingLines()
    {
        var inventoryCount = new WHMS.Domain.Entities.InventoryCount
        {
            Id = _inventoryCountId, WarehouseId = _warehouseId, IsCompleted = false, InventoryCountLines = null
        };
        _inventoryCountRepository.Setup(r => r.GetInventoryCount(_inventoryCountId)).ReturnsAsync(inventoryCount);

        await _handler.Handle(Request(), CancellationToken.None);

        _inventoryCountRepository.Verify(r => r.DeleteInventoryCount(inventoryCount), Times.Once);
        _inventoryCountRepository.Verify(r => r.DeleteInventoryCountLine(It.IsAny<InventoryCountLine>()), Times.Never);
    }
}
