using Moq;
using WHMS.Application.Abstractions.Infrastructure;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Common.Constants;
using WHMS.Application.Common.Filtering.Filters;
using WHMS.Application.Features.Command.WasteRecord.DeleteWasteRecord;
using WHMS.Domain.Exceptions;

namespace WHMS.Tests.Application.Features.Command.WasteRecord.DeleteWasteRecord;

public class DeleteWasteRecordCommandHandlerTests
{
    private readonly Mock<IWasteRecordRepository> _wasteRecordRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IInventoryCountRepository> _inventoryCountRepository = new();
    private readonly Mock<ICurrentUserService> _currentUserService = new();
    private readonly Mock<ICatalogRepository> _catalogRepository = new();
    private readonly DeleteWasteRecordCommandHandler _handler;

    private readonly Guid _wasteRecordId = Guid.NewGuid();
    private readonly Guid _warehouseId = Guid.NewGuid();
    private readonly Guid _otherWarehouseId = Guid.NewGuid();
    private readonly Guid _userId = Guid.NewGuid();

    public DeleteWasteRecordCommandHandlerTests()
    {
        _handler = new DeleteWasteRecordCommandHandler(
            _wasteRecordRepository.Object, _unitOfWork.Object, _inventoryCountRepository.Object,
            _currentUserService.Object, _catalogRepository.Object);
    }

    private WHMS.Domain.Entities.WasteRecord MakeRecord(Guid warehouseId, Guid? createdById) => new()
    {
        Id = _wasteRecordId,
        WarehouseId = warehouseId,
        SkuId = Guid.NewGuid(),
        Quantity = 3,
        CreatedAt = new DateTime(2026, 1, 1),
        CreatedById = createdById
    };

    private void SetCurrentUser(string role, string? warehouseId, Guid? userId = null)
    {
        _currentUserService.Setup(u => u.Roles).Returns([role]);
        _currentUserService.Setup(u => u.WarehouseId).Returns(warehouseId);
        _currentUserService.Setup(u => u.UserId).Returns(userId ?? _userId);
    }

    private void SetNoConflictingCounts()
    {
        _inventoryCountRepository
            .Setup(r => r.GetInventoryCounts(It.IsAny<InventoryCountFilter>(), false))
            .ReturnsAsync([]);
    }

    private DeleteWasteRecordCommandRequest Request() => new() { WasteRecordId = _wasteRecordId.ToString() };

    [Fact]
    public async Task Handle_WasteRecordNotFound_ThrowsNotFound()
    {
        _wasteRecordRepository.Setup(r => r.GetWasteRecord(_wasteRecordId)).ReturnsAsync((WHMS.Domain.Entities.WasteRecord)null!);

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(Request(), CancellationToken.None));

        Assert.Equal("Waste record not found.", exception.Message);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }

    [Theory]
    [InlineData(ApplicationRole.WarehouseManager, false, true)]   // manager, different warehouse
    [InlineData(ApplicationRole.WarehouseStaff, false, true)]     // staff, different warehouse
    [InlineData(ApplicationRole.WarehouseStaff, true, false)]     // staff, same warehouse but not the creator
    public async Task Handle_AccessDenied_ThrowsNotFound(string role, bool sameWarehouse, bool isCreator)
    {
        var recordWarehouseId = sameWarehouse ? _warehouseId : _otherWarehouseId;
        var record = MakeRecord(recordWarehouseId, isCreator ? _userId : Guid.NewGuid());
        _wasteRecordRepository.Setup(r => r.GetWasteRecord(_wasteRecordId)).ReturnsAsync(record);
        SetCurrentUser(role, _warehouseId.ToString());

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(Request(), CancellationToken.None));

        Assert.Equal("Waste record not found.", exception.Message);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_ActiveInventoryCountExistsAfterCreation_ThrowsAndDoesNotDelete()
    {
        var record = MakeRecord(_warehouseId, _userId);
        _wasteRecordRepository.Setup(r => r.GetWasteRecord(_wasteRecordId)).ReturnsAsync(record);
        SetCurrentUser(ApplicationRole.LogisticDirector, _warehouseId.ToString());
        _inventoryCountRepository
            .Setup(r => r.GetInventoryCounts(It.Is<InventoryCountFilter>(f => f.IsDone == false), false))
            .ReturnsAsync([new() { Id = Guid.NewGuid() }]);

        var exception = await Assert.ThrowsAsync<BusinessRuleViolationException>(() => _handler.Handle(Request(), CancellationToken.None));

        Assert.Equal("Cannot delete waste record while an active inventory count exists after its creation date.", exception.Message);
        _wasteRecordRepository.Verify(r => r.Delete(It.IsAny<Guid>()), Times.Never);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_CompletedInventoryCountWithSameSkuExistsAfterCreation_ThrowsAndDoesNotDelete()
    {
        var record = MakeRecord(_warehouseId, _userId);
        _wasteRecordRepository.Setup(r => r.GetWasteRecord(_wasteRecordId)).ReturnsAsync(record);
        SetCurrentUser(ApplicationRole.LogisticDirector, _warehouseId.ToString());
        _inventoryCountRepository
            .Setup(r => r.GetInventoryCounts(It.Is<InventoryCountFilter>(f => f.IsDone == false), false))
            .ReturnsAsync([]);
        _inventoryCountRepository
            .Setup(r => r.GetInventoryCounts(It.Is<InventoryCountFilter>(f =>
                f.IsDone == true && f.SkuIds!.Single() == record.SkuId.ToString()), false))
            .ReturnsAsync([new() { Id = Guid.NewGuid() }]);

        var exception = await Assert.ThrowsAsync<BusinessRuleViolationException>(() => _handler.Handle(Request(), CancellationToken.None));

        Assert.Equal("Waste record cannot be deleted because a completed inventory count including this SKU exists after its creation date.", exception.Message);
        _wasteRecordRepository.Verify(r => r.Delete(It.IsAny<Guid>()), Times.Never);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_LogisticDirector_DeletesRegardlessOfWarehouse()
    {
        var record = MakeRecord(_otherWarehouseId, Guid.NewGuid());
        _wasteRecordRepository.Setup(r => r.GetWasteRecord(_wasteRecordId)).ReturnsAsync(record);
        SetCurrentUser(ApplicationRole.LogisticDirector, warehouseId: null);
        SetNoConflictingCounts();

        await _handler.Handle(Request(), CancellationToken.None);

        _wasteRecordRepository.Verify(r => r.Delete(_wasteRecordId), Times.Once);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_ManagerSameWarehouse_DeletesSuccessfully()
    {
        var record = MakeRecord(_warehouseId, Guid.NewGuid());
        _wasteRecordRepository.Setup(r => r.GetWasteRecord(_wasteRecordId)).ReturnsAsync(record);
        SetCurrentUser(ApplicationRole.WarehouseManager, _warehouseId.ToString());
        SetNoConflictingCounts();

        await _handler.Handle(Request(), CancellationToken.None);

        _wasteRecordRepository.Verify(r => r.Delete(_wasteRecordId), Times.Once);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_StaffSameWarehouseAndCreator_DeletesSuccessfully()
    {
        var record = MakeRecord(_warehouseId, _userId);
        _wasteRecordRepository.Setup(r => r.GetWasteRecord(_wasteRecordId)).ReturnsAsync(record);
        SetCurrentUser(ApplicationRole.WarehouseStaff, _warehouseId.ToString());
        SetNoConflictingCounts();

        await _handler.Handle(Request(), CancellationToken.None);

        _wasteRecordRepository.Verify(r => r.Delete(_wasteRecordId), Times.Once);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
    }
}
