using WHMS.Domain.Exceptions;

namespace WHMS.Domain.BusinessRules.Warehouse;

// Partial for now - only what the Employee aggregate's handlers need
// (checking a referenced warehouse by id, without fetching the entity).
// Expanded when the Warehouse aggregate itself is migrated.
public static class WarehouseRules
{
    public static void EnsureExists(bool exists)
    {
        if (!exists)
            throw new NotFoundException("Warehouse not found.");
    }

    public static void EnsureNoManagerAssigned(bool hasManager)
    {
        if (hasManager)
            throw new BusinessRuleViolationException("The warehouse has already a manager.");
    }
}
