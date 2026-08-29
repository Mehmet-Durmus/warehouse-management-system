using Moq;
using WHMS.Application.Abstractions.Infrastructure;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Common.Constants;
using WHMS.Application.Features.Command.InventoryCount.UpdateInventoryCountLine;
using WHMS.Domain.Entities;
using WHMS.Domain.Exceptions;

namespace WHMS.Tests.Application.Features.Command.InventoryCount.UpdateInventoryCountLine;

public class UpdateInventoryCountLineCommandHandlerTests
{
    private readonly Mock<IInventoryCountRepository> _inventoryCountRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<ICurrentUserService> _currentUserService = new();
    private readonly Mock<IStockStateRepository> _stockStateRepository = new();
    private readonly UpdateInventoryCountLineCommandHandler _handler;
    private readonly Guid _warehouseId = Guid.NewGuid();
    private readonly Guid _inventoryCountId = Guid.NewGuid();
    private readonly Guid _lineId = Guid.NewGuid();
    private readonly Guid _skuId = Guid.NewGuid();
    private readonly Guid _userId = Guid.NewGuid();

    public UpdateInventoryCountLineCommandHandlerTests()
    {
        _currentUserService.Setup(u => u.WarehouseId).Returns(_warehouseId.ToString());
        _currentUserService.Setup(u => u.Roles).Returns([ApplicationRole.LogisticDirector]);
        _currentUserService.Setup(u => u.UserId).Returns(_userId);
        _handler = new UpdateInventoryCountLineCommandHandler(
            _inventoryCountRepository.Object, _unitOfWork.Object, _currentUserService.Object, _stockStateRepository.Object);
    }

    private WHMS.Domain.Entities.InventoryCount OpenInventoryCount(Guid id) => new() { Id = id, WarehouseId = _warehouseId, IsCompleted = false };

    private UpdateInventoryCountLineCommandRequest Request(Guid inventoryCountId, int quantity) => new()
    {
        InventoryCountId = inventoryCountId.ToString(), InventoryCountLineId = _lineId.ToString(), SkuId = _skuId.ToString(), Quantity = quantity
    };

    [Fact]
    public async Task Handle_TargetInventoryCountNotFound_ThrowsAndDoesNotCommit()
    {
        _inventoryCountRepository.Setup(r => r.GetInventoryCount(_inventoryCountId)).ReturnsAsync((WHMS.Domain.Entities.InventoryCount)null!);

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(Request(_inventoryCountId, 5), CancellationToken.None));

        Assert.Equal("Inventory count not found.", exception.Message);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_TargetInventoryCountAlreadyCompleted_ThrowsWithIdInMessage()
    {
        var completed = new WHMS.Domain.Entities.InventoryCount { Id = _inventoryCountId, WarehouseId = _warehouseId, IsCompleted = true };
        _inventoryCountRepository.Setup(r => r.GetInventoryCount(_inventoryCountId)).ReturnsAsync(completed);

        var exception = await Assert.ThrowsAsync<BusinessRuleViolationException>(() => _handler.Handle(Request(_inventoryCountId, 5), CancellationToken.None));

        Assert.Equal($"{_inventoryCountId}: Inventory count has already been completed.", exception.Message);
    }

    [Fact]
    public async Task Handle_LineNotFound_Throws()
    {
        _inventoryCountRepository.Setup(r => r.GetInventoryCount(_inventoryCountId)).ReturnsAsync(OpenInventoryCount(_inventoryCountId));
        _inventoryCountRepository.Setup(r => r.GetInventoryCountLine(_lineId)).ReturnsAsync((InventoryCountLine)null!);

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(Request(_inventoryCountId, 5), CancellationToken.None));

        Assert.Equal("Inventory count line not found.", exception.Message);
    }

    [Fact]
    public async Task Handle_StaffNotTheCreator_ThrowsSameNotFoundMessage()
    {
        _currentUserService.Setup(u => u.Roles).Returns([ApplicationRole.WarehouseStaff]);
        _inventoryCountRepository.Setup(r => r.GetInventoryCount(_inventoryCountId)).ReturnsAsync(OpenInventoryCount(_inventoryCountId));
        var line = new InventoryCountLine { Id = _lineId, InventoryCountId = _inventoryCountId, SkuId = _skuId, CreatedById = Guid.NewGuid() };
        _inventoryCountRepository.Setup(r => r.GetInventoryCountLine(_lineId)).ReturnsAsync(line);

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(Request(_inventoryCountId, 5), CancellationToken.None));

        Assert.Equal("Inventory count line not found.", exception.Message);
    }

    [Fact]
    public async Task Handle_MovedToDifferentCountAndOldCountAlreadyCompleted_ThrowsWithOldIdInMessage()
    {
        var oldCountId = Guid.NewGuid();
        _inventoryCountRepository.Setup(r => r.GetInventoryCount(_inventoryCountId)).ReturnsAsync(OpenInventoryCount(_inventoryCountId));
        var line = new InventoryCountLine { Id = _lineId, InventoryCountId = oldCountId, SkuId = _skuId, CreatedById = _userId };
        _inventoryCountRepository.Setup(r => r.GetInventoryCountLine(_lineId)).ReturnsAsync(line);
        var oldCount = new WHMS.Domain.Entities.InventoryCount { Id = oldCountId, WarehouseId = _warehouseId, IsCompleted = true };
        _inventoryCountRepository.Setup(r => r.GetInventoryCount(oldCountId)).ReturnsAsync(oldCount);

        var exception = await Assert.ThrowsAsync<BusinessRuleViolationException>(() => _handler.Handle(Request(_inventoryCountId, 5), CancellationToken.None));

        Assert.Equal($"{oldCountId}: Inventory count has already been completed.", exception.Message);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_MovedToDifferentCount_DoesNotFetchOldCountTwiceAndUpdatesLine()
    {
        var oldCountId = Guid.NewGuid();
        _inventoryCountRepository.Setup(r => r.GetInventoryCount(_inventoryCountId)).ReturnsAsync(OpenInventoryCount(_inventoryCountId));
        var line = new InventoryCountLine { Id = _lineId, InventoryCountId = oldCountId, SkuId = Guid.NewGuid(), Quantity = 3, CreatedById = _userId };
        _inventoryCountRepository.Setup(r => r.GetInventoryCountLine(_lineId)).ReturnsAsync(line);
        var oldCount = new WHMS.Domain.Entities.InventoryCount { Id = oldCountId, WarehouseId = _warehouseId, IsCompleted = false };
        _inventoryCountRepository.Setup(r => r.GetInventoryCount(oldCountId)).ReturnsAsync(oldCount);
        _stockStateRepository.Setup(r => r.GetStockQuantity(_warehouseId, _skuId)).ReturnsAsync(20);

        var response = await _handler.Handle(Request(_inventoryCountId, 15), CancellationToken.None);

        Assert.Equal(_inventoryCountId, line.InventoryCountId);
        Assert.Equal(_skuId, line.SkuId);
        Assert.Equal(15, line.Quantity);
        Assert.Equal(-5, line.Variance);
        Assert.Equal(-5, response.Variance);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_StaysInSameCount_SkipsOldCountLookupAndRecalculatesVariance()
    {
        _inventoryCountRepository.Setup(r => r.GetInventoryCount(_inventoryCountId)).ReturnsAsync(OpenInventoryCount(_inventoryCountId));
        var line = new InventoryCountLine { Id = _lineId, InventoryCountId = _inventoryCountId, SkuId = _skuId, Quantity = 3, Variance = -7, CreatedById = _userId };
        _inventoryCountRepository.Setup(r => r.GetInventoryCountLine(_lineId)).ReturnsAsync(line);
        _stockStateRepository.Setup(r => r.GetStockQuantity(_warehouseId, _skuId)).ReturnsAsync(8);

        var response = await _handler.Handle(Request(_inventoryCountId, 10), CancellationToken.None);

        _inventoryCountRepository.Verify(r => r.GetInventoryCount(_inventoryCountId), Times.Once);
        Assert.Equal(2, line.Variance);
        Assert.Equal(2, response.Variance);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
    }
}
