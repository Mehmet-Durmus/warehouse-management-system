using Moq;
using WHMS.Application.Abstractions.Infrastructure;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Features.Command.InventoryCount.CreateInventoryCountLine;
using WHMS.Domain.Entities;

namespace WHMS.Tests.Application.Features.Command.InventoryCount.CreateInventoryCountLine;

public class CreateInventoryCountLineCommandHandlerTests
{
    private readonly Mock<IInventoryCountRepository> _inventoryCountRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<ICurrentUserService> _currentUserService = new();
    private readonly Mock<IStockStateRepository> _stockStateRepository = new();
    private readonly CreateInventoryCountLineCommandHandler _handler;
    private readonly Guid _warehouseId = Guid.NewGuid();
    private readonly Guid _inventoryCountId = Guid.NewGuid();
    private readonly Guid _skuId = Guid.NewGuid();

    public CreateInventoryCountLineCommandHandlerTests()
    {
        _currentUserService.Setup(u => u.WarehouseId).Returns(_warehouseId.ToString());
        _handler = new CreateInventoryCountLineCommandHandler(
            _inventoryCountRepository.Object, _unitOfWork.Object, _currentUserService.Object, _stockStateRepository.Object);
    }

    private CreateInventoryCountLineCommandRequest Request(int quantity) => new()
    {
        InventoryCountId = _inventoryCountId.ToString(), SkuId = _skuId.ToString(), Quantity = quantity
    };

    [Fact]
    public async Task Handle_InventoryCountNotFound_ThrowsAndDoesNotCreate()
    {
        _inventoryCountRepository.Setup(r => r.GetInventoryCount(_inventoryCountId)).ReturnsAsync((WHMS.Domain.Entities.InventoryCount)null!);

        var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(Request(10), CancellationToken.None));

        Assert.Equal("Inventory count not found.", exception.Message);
        _inventoryCountRepository.Verify(r => r.CreateInventoryCounLine(It.IsAny<InventoryCountLine>()), Times.Never);
    }

    [Fact]
    public async Task Handle_InventoryCountBelongsToAnotherWarehouse_ThrowsSameNotFoundMessage()
    {
        var inventoryCount = new WHMS.Domain.Entities.InventoryCount { Id = _inventoryCountId, WarehouseId = Guid.NewGuid() };
        _inventoryCountRepository.Setup(r => r.GetInventoryCount(_inventoryCountId)).ReturnsAsync(inventoryCount);

        var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(Request(10), CancellationToken.None));

        Assert.Equal("Inventory count not found.", exception.Message);
    }

    [Fact]
    public async Task Handle_InventoryCountAlreadyCompleted_ThrowsAndDoesNotCreate()
    {
        var inventoryCount = new WHMS.Domain.Entities.InventoryCount { Id = _inventoryCountId, WarehouseId = _warehouseId, IsCompleted = true };
        _inventoryCountRepository.Setup(r => r.GetInventoryCount(_inventoryCountId)).ReturnsAsync(inventoryCount);

        var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(Request(10), CancellationToken.None));

        Assert.Equal("This inventory count already completed.", exception.Message);
        _inventoryCountRepository.Verify(r => r.CreateInventoryCounLine(It.IsAny<InventoryCountLine>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ValidRequest_ComputesVarianceAgainstCurrentStockAndReturnsResponse()
    {
        var inventoryCount = new WHMS.Domain.Entities.InventoryCount { Id = _inventoryCountId, WarehouseId = _warehouseId, IsCompleted = false };
        _inventoryCountRepository.Setup(r => r.GetInventoryCount(_inventoryCountId)).ReturnsAsync(inventoryCount);
        _stockStateRepository.Setup(r => r.GetStockQuantity(_warehouseId, _skuId)).ReturnsAsync(15);

        var response = await _handler.Handle(Request(12), CancellationToken.None);

        _inventoryCountRepository.Verify(r => r.CreateInventoryCounLine(It.Is<InventoryCountLine>(l =>
            l.InventoryCountId == _inventoryCountId && l.SkuId == _skuId && l.Quantity == 12 && l.Variance == -3)), Times.Once);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        Assert.Equal(12, response.Quantity);
        Assert.Equal(-3, response.Variance);
    }
}
