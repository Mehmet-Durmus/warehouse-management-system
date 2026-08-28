using WHMS.Domain.Exceptions;

namespace WHMS.Domain.BusinessRules.WasteRecord;

public static class WasteRecordRules
{
    // Role membership is passed in as booleans (rather than an ApplicationRole
    // constant) because WHMS.Domain must not depend on WHMS.Application - the
    // handler resolves "does the caller have this role" and hands the answer down.
    public static WHMS.Domain.Entities.WasteRecord EnsureAccessible(
        WHMS.Domain.Entities.WasteRecord? wasteRecord,
        bool currentUserIsManager,
        bool currentUserIsStaff,
        Guid? currentUserWarehouseId,
        Guid? currentUserId)
    {
        bool notFound = wasteRecord switch
        {
            null => true,
            not null when currentUserIsManager && wasteRecord.WarehouseId != currentUserWarehouseId => true,
            not null when currentUserIsStaff
                && (wasteRecord.WarehouseId != currentUserWarehouseId || wasteRecord.CreatedById != currentUserId) => true,
            _ => false
        };

        if (notFound)
            throw new NotFoundException("Waste record not found.");

        return wasteRecord!;
    }

    public static void EnsureSufficientStock(bool wasSufficient)
    {
        if (!wasSufficient)
            throw new BusinessRuleViolationException("Insufficient stock exception.");
    }

    public static void EnsureNoActiveInventoryCountBlocksDeletion(bool activeInventoryCountExists)
    {
        if (activeInventoryCountExists)
            throw new BusinessRuleViolationException(
                "Cannot delete waste record while an active inventory count exists after its creation date.");
    }

    public static void EnsureNoCompletedInventoryCountWithSkuBlocksDeletion(bool completedInventoryCountExists)
    {
        if (completedInventoryCountExists)
            throw new BusinessRuleViolationException(
                "Waste record cannot be deleted because a completed inventory count including this SKU exists after its creation date.");
    }

    public static void EnsureNoActiveInventoryCountBlocksUpdate(bool activeInventoryCountExists)
    {
        if (activeInventoryCountExists)
            throw new BusinessRuleViolationException(
                "Cannot update waste record while an active inventory count exists after its creation date.");
    }

    public static void EnsureNoCompletedInventoryCountWithSkuBlocksUpdate(bool completedInventoryCountExists)
    {
        if (completedInventoryCountExists)
            throw new BusinessRuleViolationException(
                "Waste record cannot be updated because a completed inventory count including this SKU exists after its creation date.");
    }
}
