using WHMS.Domain.Entities;
using WHMS.Domain.Exceptions;

namespace WHMS.Domain.BusinessRules.Employee;

public static class EmployeeRules
{
    public static ApplicationUser EnsureExists(ApplicationUser? employee)
    {
        if (employee is null)
            throw new NotFoundException("Employee not found.");
        return employee;
    }

    public static ApplicationUser EnsureManagerExists(ApplicationUser? manager)
    {
        if (manager is null)
            throw new NotFoundException("Manager not found.");
        return manager;
    }

    public static ApplicationUser EnsureStaffMemberExists(ApplicationUser? staffMember)
    {
        if (staffMember is null)
            throw new NotFoundException("Staff member not found.");
        return staffMember;
    }

    // Combines "does this employee exist" with "is this a self-service attempt" -
    // a user resetting their own password through this (admin-only) path is
    // treated the same as the employee not existing.
    public static ApplicationUser EnsureCanResetPassword(ApplicationUser? employee, Guid? currentUserId)
    {
        if (employee is null || employee.Id == currentUserId)
            throw new NotFoundException("Employee not found.");
        return employee;
    }

    public static void EnsureManagerWasCreated(bool succeeded)
    {
        if (!succeeded)
            throw new BusinessRuleViolationException("Manager could not be created.");
    }

    public static void EnsureStaffMemberWasCreated(bool succeeded)
    {
        if (!succeeded)
            throw new BusinessRuleViolationException("Staff member could not be created.");
    }
}
