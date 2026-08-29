using WHMS.Domain.Entities;
using WHMS.Domain.Exceptions;

namespace WHMS.Domain.BusinessRules.InventoryCount;

public static class InventoryCountRules
{
    // Combines the null-check with the warehouse-ownership check shared by every
    // handler that looks up an inventory count by id.
    public static WHMS.Domain.Entities.InventoryCount EnsureAccessibleForWarehouse(WHMS.Domain.Entities.InventoryCount? inventoryCount, Guid warehouseId)
    {
        if (inventoryCount is null || inventoryCount.WarehouseId != warehouseId)
            throw new NotFoundException("Inventory count not found.");
        return inventoryCount;
    }

    public static void EnsureCreationAllowed(bool isThereUncompletedInventoryCount)
    {
        if (isThereUncompletedInventoryCount)
            throw new BusinessRuleViolationException("There is uncomplated inventory count.");
    }

    // Preserves CreateInventoryCountLine's own distinct phrasing.
    public static void EnsureNotCompletedForLineCreation(bool isCompleted)
    {
        if (isCompleted)
            throw new BusinessRuleViolationException("This inventory count already completed.");
    }

    public static void EnsureNotCompleted(bool isCompleted)
    {
        if (isCompleted)
            throw new BusinessRuleViolationException("Inventory count has already been completed.");
    }

    // Preserves UpdateInventoryCountLine's id-qualified phrasing, used both for the
    // target count and, when a line moves between counts, the original count.
    public static void EnsureNotCompletedForLineUpdate(bool isCompleted, Guid inventoryCountId)
    {
        if (isCompleted)
            throw new BusinessRuleViolationException($"{inventoryCountId}: Inventory count has already been completed.");
    }

    // Combines the null-check with the "staff may only touch their own lines" rule;
    // Logistic Directors and Warehouse Managers are unrestricted.
    public static InventoryCountLine EnsureLineAccessible(InventoryCountLine? line, bool restrictedToOwnLines, Guid? currentUserId)
    {
        if (line is null || (restrictedToOwnLines && line.CreatedById != currentUserId))
            throw new NotFoundException("Inventory count line not found.");
        return line;
    }

    // Pure computation, no throw.
    public static int CalculateVariance(int quantity, int currentStock) => quantity - currentStock;
}
