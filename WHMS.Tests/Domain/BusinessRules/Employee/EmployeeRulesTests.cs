using WHMS.Domain.BusinessRules.Employee;
using WHMS.Domain.Entities;
using WHMS.Domain.Exceptions;

namespace WHMS.Tests.Domain.BusinessRules.Employee;

public class EmployeeRulesTests
{
    private static ApplicationUser MakeUser(Guid id) => new() { Id = id, UserName = "WHS_0001", FullName = "Test" };

    [Fact]
    public void EnsureExists_EmployeeIsNull_ThrowsNotFoundException()
    {
        var exception = Assert.Throws<NotFoundException>(() => EmployeeRules.EnsureExists(null));

        Assert.Equal("Employee not found.", exception.Message);
    }

    [Fact]
    public void EnsureExists_EmployeeIsNotNull_ReturnsTheSameEmployee()
    {
        var user = MakeUser(Guid.NewGuid());

        var result = EmployeeRules.EnsureExists(user);

        Assert.Same(user, result);
    }

    [Fact]
    public void EnsureManagerExists_ManagerIsNull_ThrowsNotFoundException()
    {
        var exception = Assert.Throws<NotFoundException>(() => EmployeeRules.EnsureManagerExists(null));

        Assert.Equal("Manager not found.", exception.Message);
    }

    [Fact]
    public void EnsureStaffMemberExists_StaffMemberIsNull_ThrowsNotFoundException()
    {
        var exception = Assert.Throws<NotFoundException>(() => EmployeeRules.EnsureStaffMemberExists(null));

        Assert.Equal("Staff member not found.", exception.Message);
    }

    [Fact]
    public void EnsureCanResetPassword_EmployeeIsNull_ThrowsNotFoundException()
    {
        var exception = Assert.Throws<NotFoundException>(() => EmployeeRules.EnsureCanResetPassword(null, Guid.NewGuid()));

        Assert.Equal("Employee not found.", exception.Message);
    }

    [Fact]
    public void EnsureCanResetPassword_TargetIsTheCurrentUser_ThrowsNotFoundException()
    {
        var userId = Guid.NewGuid();
        var user = MakeUser(userId);

        var exception = Assert.Throws<NotFoundException>(() => EmployeeRules.EnsureCanResetPassword(user, userId));

        Assert.Equal("Employee not found.", exception.Message);
    }

    [Fact]
    public void EnsureCanResetPassword_TargetIsSomeoneElse_ReturnsTheSameEmployee()
    {
        var user = MakeUser(Guid.NewGuid());

        var result = EmployeeRules.EnsureCanResetPassword(user, Guid.NewGuid());

        Assert.Same(user, result);
    }

    [Fact]
    public void EnsureManagerWasCreated_NotSucceeded_ThrowsBusinessRuleViolationException()
    {
        var exception = Assert.Throws<BusinessRuleViolationException>(() => EmployeeRules.EnsureManagerWasCreated(succeeded: false));

        Assert.Equal("Manager could not be created.", exception.Message);
    }

    [Fact]
    public void EnsureStaffMemberWasCreated_NotSucceeded_ThrowsBusinessRuleViolationException()
    {
        var exception = Assert.Throws<BusinessRuleViolationException>(() => EmployeeRules.EnsureStaffMemberWasCreated(succeeded: false));

        Assert.Equal("Staff member could not be created.", exception.Message);
    }
}
