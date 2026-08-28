using WHMS.Domain.BusinessRules.Warehouse;
using WHMS.Domain.Exceptions;

namespace WHMS.Tests.Domain.BusinessRules.Warehouse;

public class WarehouseRulesTests
{
    [Fact]
    public void EnsureExists_DoesNotExist_ThrowsNotFoundException()
    {
        var exception = Assert.Throws<NotFoundException>(() => WarehouseRules.EnsureExists(exists: false));

        Assert.Equal("Warehouse not found.", exception.Message);
    }

    [Fact]
    public void EnsureExists_Exists_DoesNotThrow()
    {
        var exception = Record.Exception(() => WarehouseRules.EnsureExists(exists: true));

        Assert.Null(exception);
    }

    [Fact]
    public void EnsureNoManagerAssigned_HasManager_ThrowsBusinessRuleViolationException()
    {
        var exception = Assert.Throws<BusinessRuleViolationException>(() => WarehouseRules.EnsureNoManagerAssigned(hasManager: true));

        Assert.Equal("The warehouse has already a manager.", exception.Message);
    }

    [Fact]
    public void EnsureNoManagerAssigned_NoManager_DoesNotThrow()
    {
        var exception = Record.Exception(() => WarehouseRules.EnsureNoManagerAssigned(hasManager: false));

        Assert.Null(exception);
    }
}
