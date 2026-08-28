using WHMS.Domain.BusinessRules.WasteRecord;
using WHMS.Domain.Exceptions;

namespace WHMS.Tests.Domain.BusinessRules.WasteRecord;

public class WasteRecordRulesTests
{
    private readonly Guid _warehouseId = Guid.NewGuid();
    private readonly Guid _otherWarehouseId = Guid.NewGuid();
    private readonly Guid _userId = Guid.NewGuid();

    private WHMS.Domain.Entities.WasteRecord MakeRecord(Guid warehouseId, Guid? createdById) => new()
    {
        WarehouseId = warehouseId,
        SkuId = Guid.NewGuid(),
        Quantity = 1,
        CreatedById = createdById
    };

    [Fact]
    public void EnsureAccessible_RecordIsNull_ThrowsNotFoundException()
    {
        var exception = Assert.Throws<NotFoundException>(() =>
            WasteRecordRules.EnsureAccessible(null, currentUserIsManager: false, currentUserIsStaff: false, _warehouseId, _userId));

        Assert.Equal("Waste record not found.", exception.Message);
    }

    [Fact]
    public void EnsureAccessible_ManagerFromAnotherWarehouse_ThrowsNotFoundException()
    {
        var record = MakeRecord(_otherWarehouseId, Guid.NewGuid());

        var exception = Assert.Throws<NotFoundException>(() =>
            WasteRecordRules.EnsureAccessible(record, currentUserIsManager: true, currentUserIsStaff: false, _warehouseId, _userId));

        Assert.Equal("Waste record not found.", exception.Message);
    }

    [Fact]
    public void EnsureAccessible_StaffFromAnotherWarehouse_ThrowsNotFoundException()
    {
        var record = MakeRecord(_otherWarehouseId, _userId);

        var exception = Assert.Throws<NotFoundException>(() =>
            WasteRecordRules.EnsureAccessible(record, currentUserIsManager: false, currentUserIsStaff: true, _warehouseId, _userId));

        Assert.Equal("Waste record not found.", exception.Message);
    }

    [Fact]
    public void EnsureAccessible_StaffSameWarehouseButNotTheCreator_ThrowsNotFoundException()
    {
        var record = MakeRecord(_warehouseId, Guid.NewGuid());

        var exception = Assert.Throws<NotFoundException>(() =>
            WasteRecordRules.EnsureAccessible(record, currentUserIsManager: false, currentUserIsStaff: true, _warehouseId, _userId));

        Assert.Equal("Waste record not found.", exception.Message);
    }

    [Fact]
    public void EnsureAccessible_ManagerSameWarehouse_ReturnsTheSameRecord()
    {
        var record = MakeRecord(_warehouseId, Guid.NewGuid());

        var result = WasteRecordRules.EnsureAccessible(record, currentUserIsManager: true, currentUserIsStaff: false, _warehouseId, _userId);

        Assert.Same(record, result);
    }

    [Fact]
    public void EnsureAccessible_StaffSameWarehouseAndCreator_ReturnsTheSameRecord()
    {
        var record = MakeRecord(_warehouseId, _userId);

        var result = WasteRecordRules.EnsureAccessible(record, currentUserIsManager: false, currentUserIsStaff: true, _warehouseId, _userId);

        Assert.Same(record, result);
    }

    [Fact]
    public void EnsureAccessible_NeitherManagerNorStaff_BypassesWarehouseScoping()
    {
        var record = MakeRecord(_otherWarehouseId, Guid.NewGuid());

        var result = WasteRecordRules.EnsureAccessible(record, currentUserIsManager: false, currentUserIsStaff: false, _warehouseId, _userId);

        Assert.Same(record, result);
    }

    [Fact]
    public void EnsureSufficientStock_WasNotSufficient_ThrowsBusinessRuleViolationException()
    {
        var exception = Assert.Throws<BusinessRuleViolationException>(() => WasteRecordRules.EnsureSufficientStock(wasSufficient: false));

        Assert.Equal("Insufficient stock exception.", exception.Message);
    }

    [Fact]
    public void EnsureSufficientStock_WasSufficient_DoesNotThrow()
    {
        var exception = Record.Exception(() => WasteRecordRules.EnsureSufficientStock(wasSufficient: true));

        Assert.Null(exception);
    }

    [Fact]
    public void EnsureNoActiveInventoryCountBlocksDeletion_Exists_Throws()
    {
        var exception = Assert.Throws<BusinessRuleViolationException>(
            () => WasteRecordRules.EnsureNoActiveInventoryCountBlocksDeletion(activeInventoryCountExists: true));

        Assert.Equal("Cannot delete waste record while an active inventory count exists after its creation date.", exception.Message);
    }

    [Fact]
    public void EnsureNoCompletedInventoryCountWithSkuBlocksDeletion_Exists_Throws()
    {
        var exception = Assert.Throws<BusinessRuleViolationException>(
            () => WasteRecordRules.EnsureNoCompletedInventoryCountWithSkuBlocksDeletion(completedInventoryCountExists: true));

        Assert.Equal(
            "Waste record cannot be deleted because a completed inventory count including this SKU exists after its creation date.",
            exception.Message);
    }

    [Fact]
    public void EnsureNoActiveInventoryCountBlocksUpdate_Exists_Throws()
    {
        var exception = Assert.Throws<BusinessRuleViolationException>(
            () => WasteRecordRules.EnsureNoActiveInventoryCountBlocksUpdate(activeInventoryCountExists: true));

        Assert.Equal("Cannot update waste record while an active inventory count exists after its creation date.", exception.Message);
    }

    [Fact]
    public void EnsureNoCompletedInventoryCountWithSkuBlocksUpdate_Exists_Throws()
    {
        var exception = Assert.Throws<BusinessRuleViolationException>(
            () => WasteRecordRules.EnsureNoCompletedInventoryCountWithSkuBlocksUpdate(completedInventoryCountExists: true));

        Assert.Equal(
            "Waste record cannot be updated because a completed inventory count including this SKU exists after its creation date.",
            exception.Message);
    }
}
