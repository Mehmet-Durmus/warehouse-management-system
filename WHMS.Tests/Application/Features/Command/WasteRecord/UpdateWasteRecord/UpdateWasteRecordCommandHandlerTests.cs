using Moq;
using WHMS.Application.Abstractions.Infrastructure;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Common.Constants;
using WHMS.Application.Common.Filtering.Filters;
using WHMS.Application.Features.Command.WasteRecord.UpdateWasteRecord;
using WHMS.Domain.Entities;
using WHMS.Domain.Exceptions;

namespace WHMS.Tests.Application.Features.Command.WasteRecord.UpdateWasteRecord;

// Note: this handler never touches IStockStateRepository (it is not even injected),
// so changing a waste record's SKU or quantity here does not re-adjust stock -
// the original deduction made at creation time is left as-is. Documenting this as
// a design question to flag, not fixing it as part of this test pass.
public class UpdateWasteRecordCommandHandlerTests
{
    private readonly Mock<IWasteRecordRepository> _wasteRecordRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IInventoryCountRepository> _inventoryCountRepository = new();
    private readonly Mock<ICurrentUserService> _currentUserService = new();
    private readonly Mock<ICatalogRepository> _catalogRepository = new();
    private readonly UpdateWasteRecordCommandHandler _handler;

    private readonly Guid _wasteRecordId = Guid.NewGuid();
    private readonly Guid _warehouseId = Guid.NewGuid();
    private readonly Guid _otherWarehouseId = Guid.NewGuid();
    private readonly Guid _userId = Guid.NewGuid();
    private readonly Guid _newSkuId = Guid.NewGuid();

    public UpdateWasteRecordCommandHandlerTests()
    {
        _handler = new UpdateWasteRecordCommandHandler(
            _wasteRecordRepository.Object, _unitOfWork.Object, _inventoryCountRepository.Object,
            _currentUserService.Object, _catalogRepository.Object);
    }

    private WHMS.Domain.Entities.WasteRecord MakeRecord(Guid warehouseId, Guid? createdById) => new()
    {
        Id = _wasteRecordId,
        WarehouseId = warehouseId,
        SkuId = Guid.NewGuid(),
        Quantity = 3,
        Description = "eski",
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

    private UpdateWasteRecordCommandRequest Request() => new()
    {
        WasteRecordId = _wasteRecordId.ToString(),
        SkuId = _newSkuId.ToString(),
        Quantity = 7,
        Description = "yeni"
    };

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
    public async Task Handle_ActiveInventoryCountExists_ThrowsAndLeavesRecordUnchanged()
    {
        var record = MakeRecord(_warehouseId, _userId);
        _wasteRecordRepository.Setup(r => r.GetWasteRecord(_wasteRecordId)).ReturnsAsync(record);
        SetCurrentUser(ApplicationRole.LogisticDirector, _warehouseId.ToString());
        _inventoryCountRepository
            .Setup(r => r.GetInventoryCounts(It.Is<InventoryCountFilter>(f => f.IsDone == false), false))
            .ReturnsAsync([new() { Id = Guid.NewGuid() }]);

        var exception = await Assert.ThrowsAsync<BusinessRuleViolationException>(() => _handler.Handle(Request(), CancellationToken.None));

        Assert.Equal("Cannot update waste record while an active inventory count exists after its creation date.", exception.Message);
        Assert.Equal("eski", record.Description);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_CompletedInventoryCountWithNewSkuExists_ThrowsUsingRequestedSkuNotOriginal()
    {
        var record = MakeRecord(_warehouseId, _userId);
        _wasteRecordRepository.Setup(r => r.GetWasteRecord(_wasteRecordId)).ReturnsAsync(record);
        SetCurrentUser(ApplicationRole.LogisticDirector, _warehouseId.ToString());
        _inventoryCountRepository
            .Setup(r => r.GetInventoryCounts(It.Is<InventoryCountFilter>(f => f.IsDone == false), false))
            .ReturnsAsync([]);
        // The completed-count check filters by request.SkuId (the *new* SKU being
        // moved to), not the waste record's current SkuId.
        _inventoryCountRepository
            .Setup(r => r.GetInventoryCounts(It.Is<InventoryCountFilter>(f =>
                f.IsDone == true && f.SkuIds!.Single() == _newSkuId.ToString()), false))
            .ReturnsAsync([new() { Id = Guid.NewGuid() }]);

        var exception = await Assert.ThrowsAsync<BusinessRuleViolationException>(() => _handler.Handle(Request(), CancellationToken.None));

        Assert.Equal("Waste record cannot be updated because a completed inventory count including this SKU exists after its creation date.", exception.Message);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_NewSkuNotFound_ThrowsAndLeavesRecordUnchanged()
    {
        var record = MakeRecord(_warehouseId, _userId);
        var originalSkuId = record.SkuId;
        _wasteRecordRepository.Setup(r => r.GetWasteRecord(_wasteRecordId)).ReturnsAsync(record);
        SetCurrentUser(ApplicationRole.LogisticDirector, _warehouseId.ToString());
        SetNoConflictingCounts();
        _catalogRepository.Setup(r => r.GetSku(_newSkuId)).ReturnsAsync((SKU)null!);

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(Request(), CancellationToken.None));

        Assert.Equal("Sku not found.", exception.Message);
        Assert.Equal(originalSkuId, record.SkuId);
        Assert.Equal(3, record.Quantity);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_ValidUpdate_UpdatesFieldsAndReturnsResponse()
    {
        var record = MakeRecord(_warehouseId, _userId);
        var newSku = new SKU { Id = _newSkuId, SKUName = "meyve suyu", NormalizedSKUName = "MEYVE SUYU", Barcode = "8690000000002" };
        _wasteRecordRepository.Setup(r => r.GetWasteRecord(_wasteRecordId)).ReturnsAsync(record);
        SetCurrentUser(ApplicationRole.LogisticDirector, _warehouseId.ToString());
        SetNoConflictingCounts();
        _catalogRepository.Setup(r => r.GetSku(_newSkuId)).ReturnsAsync(newSku);

        var response = await _handler.Handle(Request(), CancellationToken.None);

        Assert.Equal(_newSkuId, record.SkuId);
        Assert.Equal(7, record.Quantity);
        Assert.Equal("yeni", record.Description);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        Assert.Equal("meyve suyu", response.SkuName);
        Assert.Equal(7, response.Quantity);
    }
}
