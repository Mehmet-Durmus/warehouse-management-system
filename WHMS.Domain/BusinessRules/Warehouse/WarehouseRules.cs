using WHMS.Domain.Entities;
using WHMS.Domain.Exceptions;

namespace WHMS.Domain.BusinessRules.Warehouse;

public static class WarehouseRules
{
    // Used where only an existence check was fetched (no entity in hand) - e.g.
    // the Employee aggregate's handlers, which check a referenced warehouse by id.
    public static void EnsureExists(bool exists)
    {
        if (!exists)
            throw new NotFoundException("Warehouse not found.");
    }

    public static WHMS.Domain.Entities.Warehouse EnsureExists(WHMS.Domain.Entities.Warehouse? warehouse)
    {
        if (warehouse is null)
            throw new NotFoundException("Warehouse not found.");
        return warehouse;
    }

    public static void EnsureNameIsUnique(bool nameExists)
    {
        if (nameExists)
            throw new BusinessRuleViolationException("This warehouse name is being used for another warehouse.");
    }

    public static void EnsureNoManagerAssigned(bool hasManager)
    {
        if (hasManager)
            throw new BusinessRuleViolationException("The warehouse has already a manager.");
    }

    public static void EnsureCanBeDeleted(bool hasPendingDeliveries)
    {
        if (hasPendingDeliveries)
            throw new BusinessRuleViolationException(
                "Warehouse cannot be deleted because there are pending deliveries assigned to it.");
    }

    // Non-throwing: returns a warning message if the candidate can't be assigned as
    // manager, or null if the assignment is valid. The caller performs the actual
    // WarehouseId mutation, consistent with every other Rule in this codebase never
    // mutating state itself - only deciding pass/fail.
    public static string? ValidateManagerAssignment(ApplicationUser? user, bool hasManagerRole, string requestedId)
    {
        if (user is null)
            return $"'{requestedId}' is invalid. Manager not found.";
        if (!hasManagerRole)
            return $"{user.FullName} is not a manager.";
        if (user.WarehouseId is not null)
            return $"{user.FullName} works at a different warehouse.";
        return null;
    }

    public static string? ValidateStaffAssignment(ApplicationUser? user, bool hasStaffRole, string requestedId)
    {
        if (user is null)
            return $"'{requestedId}' is invalid. Staff member not found.";
        if (!hasStaffRole)
            return $"{user.FullName} is not a staff member.";
        if (user.WarehouseId is not null)
            return $"{user.FullName} works at a different warehouse.";
        return null;
    }
}
