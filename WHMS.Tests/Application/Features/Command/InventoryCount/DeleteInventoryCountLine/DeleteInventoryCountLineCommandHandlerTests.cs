using Moq;
using WHMS.Application.Abstractions.Infrastructure;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Common.Constants;
using WHMS.Application.Features.Command.InventoryCount.DeleteInventoryCountLine;
using WHMS.Domain.Entities;
using WHMS.Domain.Exceptions;

namespace WHMS.Tests.Application.Features.Command.InventoryCount.DeleteInventoryCountLine;

public class DeleteInventoryCountLineCommandHandlerTests
{
    private readonly Mock<IInventoryCountRepository> _inventoryCountRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<ICurrentUserService> _currentUserService = new();
    private readonly DeleteInventoryCountLineCommandHandler _handler;
    private readonly Guid _warehouseId = Guid.NewGuid();
    private readonly Guid _inventoryCountId = Guid.NewGuid();
    private readonly Guid _lineId = Guid.NewGuid();
    private readonly Guid _userId = Guid.NewGuid();

    public DeleteInventoryCountLineCommandHandlerTests()
    {
        _handler = new DeleteInventoryCountLineCommandHandler(_inventoryCountRepository.Object, _unitOfWork.Object, _currentUserService.Object);
    }

    private void SetCurrentUser(string role, Guid? userId = null)
    {
        _currentUserService.Setup(u => u.Roles).Returns([role]);
        _currentUserService.Setup(u => u.WarehouseId).Returns(_warehouseId.ToString());
        _currentUserService.Setup(u => u.UserId).Returns(userId ?? _userId);
    }

    private DeleteInventoryCountLineCommandRequest Request() => new() { InventoryCountLineId = _lineId.ToString() };

    [Fact]
    public async Task Handle_LineNotFound_ThrowsAndDoesNotCommit()
    {
        SetCurrentUser(ApplicationRole.LogisticDirector);
        _inventoryCountRepository.Setup(r => r.GetInventoryCountLine(_lineId)).ReturnsAsync((InventoryCountLine)null!);

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(Request(), CancellationToken.None));

        Assert.Equal("Inventory count line not found.", exception.Message);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_StaffNotTheCreator_ThrowsSameNotFoundMessage()
    {
        SetCurrentUser(ApplicationRole.WarehouseStaff);
        var line = new InventoryCountLine { Id = _lineId, InventoryCountId = _inventoryCountId, SkuId = Guid.NewGuid(), CreatedById = Guid.NewGuid() };
        _inventoryCountRepository.Setup(r => r.GetInventoryCountLine(_lineId)).ReturnsAsync(line);

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(Request(), CancellationToken.None));

        Assert.Equal("Inventory count line not found.", exception.Message);
        _inventoryCountRepository.Verify(r => r.GetInventoryCount(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ParentInventoryCountAlreadyCompleted_ThrowsAndDoesNotDelete()
    {
        SetCurrentUser(ApplicationRole.LogisticDirector);
        var line = new InventoryCountLine { Id = _lineId, InventoryCountId = _inventoryCountId, SkuId = Guid.NewGuid(), CreatedById = Guid.NewGuid() };
        var inventoryCount = new WHMS.Domain.Entities.InventoryCount { Id = _inventoryCountId, WarehouseId = _warehouseId, IsCompleted = true };
        _inventoryCountRepository.Setup(r => r.GetInventoryCountLine(_lineId)).ReturnsAsync(line);
        _inventoryCountRepository.Setup(r => r.GetInventoryCount(_inventoryCountId)).ReturnsAsync(inventoryCount);

        var exception = await Assert.ThrowsAsync<BusinessRuleViolationException>(() => _handler.Handle(Request(), CancellationToken.None));

        Assert.Equal("Inventory count has already been completed.", exception.Message);
        _inventoryCountRepository.Verify(r => r.DeleteInventoryCountLine(It.IsAny<InventoryCountLine>()), Times.Never);
    }

    [Fact]
    public async Task Handle_StaffIsTheCreator_DeletesAndCommits()
    {
        SetCurrentUser(ApplicationRole.WarehouseStaff);
        var line = new InventoryCountLine { Id = _lineId, InventoryCountId = _inventoryCountId, SkuId = Guid.NewGuid(), CreatedById = _userId };
        var inventoryCount = new WHMS.Domain.Entities.InventoryCount { Id = _inventoryCountId, WarehouseId = _warehouseId, IsCompleted = false };
        _inventoryCountRepository.Setup(r => r.GetInventoryCountLine(_lineId)).ReturnsAsync(line);
        _inventoryCountRepository.Setup(r => r.GetInventoryCount(_inventoryCountId)).ReturnsAsync(inventoryCount);

        await _handler.Handle(Request(), CancellationToken.None);

        _inventoryCountRepository.Verify(r => r.DeleteInventoryCountLine(line), Times.Once);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_ManagerNotTheCreator_StillAllowedToDelete()
    {
        SetCurrentUser(ApplicationRole.WarehouseManager);
        var line = new InventoryCountLine { Id = _lineId, InventoryCountId = _inventoryCountId, SkuId = Guid.NewGuid(), CreatedById = Guid.NewGuid() };
        var inventoryCount = new WHMS.Domain.Entities.InventoryCount { Id = _inventoryCountId, WarehouseId = _warehouseId, IsCompleted = false };
        _inventoryCountRepository.Setup(r => r.GetInventoryCountLine(_lineId)).ReturnsAsync(line);
        _inventoryCountRepository.Setup(r => r.GetInventoryCount(_inventoryCountId)).ReturnsAsync(inventoryCount);

        await _handler.Handle(Request(), CancellationToken.None);

        _inventoryCountRepository.Verify(r => r.DeleteInventoryCountLine(line), Times.Once);
    }
}
