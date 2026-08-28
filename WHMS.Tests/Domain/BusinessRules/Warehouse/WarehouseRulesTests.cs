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

    [Fact]
    public void EnsureExistsEntity_WarehouseIsNull_ThrowsNotFoundException()
    {
        var exception = Assert.Throws<NotFoundException>(() => WarehouseRules.EnsureExists((WHMS.Domain.Entities.Warehouse?)null));

        Assert.Equal("Warehouse not found.", exception.Message);
    }

    [Fact]
    public void EnsureExistsEntity_WarehouseIsNotNull_ReturnsTheSameWarehouse()
    {
        var warehouse = new WHMS.Domain.Entities.Warehouse
        {
            WarehouseName = "Depo",
            Address = new WHMS.Domain.ValueObjects.Address(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "34000", "Cadde")
        };

        var result = WarehouseRules.EnsureExists(warehouse);

        Assert.Same(warehouse, result);
    }

    [Fact]
    public void EnsureNameIsUnique_NameExists_ThrowsBusinessRuleViolationException()
    {
        var exception = Assert.Throws<BusinessRuleViolationException>(() => WarehouseRules.EnsureNameIsUnique(nameExists: true));

        Assert.Equal("This warehouse name is being used for another warehouse.", exception.Message);
    }

    [Fact]
    public void EnsureNameIsUnique_NameDoesNotExist_DoesNotThrow()
    {
        var exception = Record.Exception(() => WarehouseRules.EnsureNameIsUnique(nameExists: false));

        Assert.Null(exception);
    }

    [Fact]
    public void EnsureCanBeDeleted_HasPendingDeliveries_ThrowsBusinessRuleViolationException()
    {
        var exception = Assert.Throws<BusinessRuleViolationException>(() => WarehouseRules.EnsureCanBeDeleted(hasPendingDeliveries: true));

        Assert.Equal("Warehouse cannot be deleted because there are pending deliveries assigned to it.", exception.Message);
    }

    [Fact]
    public void EnsureCanBeDeleted_NoPendingDeliveries_DoesNotThrow()
    {
        var exception = Record.Exception(() => WarehouseRules.EnsureCanBeDeleted(hasPendingDeliveries: false));

        Assert.Null(exception);
    }

    [Fact]
    public void ValidateManagerAssignment_UserIsNull_ReturnsNotFoundWarning()
    {
        var warning = WarehouseRules.ValidateManagerAssignment(null, hasManagerRole: false, requestedId: "abc");

        Assert.Equal("'abc' is invalid. Manager not found.", warning);
    }

    [Fact]
    public void ValidateManagerAssignment_UserLacksManagerRole_ReturnsRoleWarning()
    {
        var user = new WHMS.Domain.Entities.ApplicationUser { UserName = "WHS_0001", FullName = "Test User" };

        var warning = WarehouseRules.ValidateManagerAssignment(user, hasManagerRole: false, requestedId: "abc");

        Assert.Equal("Test User is not a manager.", warning);
    }

    [Fact]
    public void ValidateManagerAssignment_UserAlreadyAssignedElsewhere_ReturnsWarehouseWarning()
    {
        var user = new WHMS.Domain.Entities.ApplicationUser { UserName = "WHM_0001", FullName = "Test User", WarehouseId = Guid.NewGuid() };

        var warning = WarehouseRules.ValidateManagerAssignment(user, hasManagerRole: true, requestedId: "abc");

        Assert.Equal("Test User works at a different warehouse.", warning);
    }

    [Fact]
    public void ValidateManagerAssignment_Valid_ReturnsNull()
    {
        var user = new WHMS.Domain.Entities.ApplicationUser { UserName = "WHM_0001", FullName = "Test User" };

        var warning = WarehouseRules.ValidateManagerAssignment(user, hasManagerRole: true, requestedId: "abc");

        Assert.Null(warning);
    }

    [Fact]
    public void ValidateStaffAssignment_UserIsNull_ReturnsNotFoundWarning()
    {
        var warning = WarehouseRules.ValidateStaffAssignment(null, hasStaffRole: false, requestedId: "abc");

        Assert.Equal("'abc' is invalid. Staff member not found.", warning);
    }

    [Fact]
    public void ValidateStaffAssignment_UserLacksStaffRole_ReturnsRoleWarning()
    {
        var user = new WHMS.Domain.Entities.ApplicationUser { UserName = "WHM_0001", FullName = "Test User" };

        var warning = WarehouseRules.ValidateStaffAssignment(user, hasStaffRole: false, requestedId: "abc");

        Assert.Equal("Test User is not a staff member.", warning);
    }

    [Fact]
    public void ValidateStaffAssignment_UserAlreadyAssignedElsewhere_ReturnsWarehouseWarning()
    {
        var user = new WHMS.Domain.Entities.ApplicationUser { UserName = "WHS_0001", FullName = "Test User", WarehouseId = Guid.NewGuid() };

        var warning = WarehouseRules.ValidateStaffAssignment(user, hasStaffRole: true, requestedId: "abc");

        Assert.Equal("Test User works at a different warehouse.", warning);
    }

    [Fact]
    public void ValidateStaffAssignment_Valid_ReturnsNull()
    {
        var user = new WHMS.Domain.Entities.ApplicationUser { UserName = "WHS_0001", FullName = "Test User" };

        var warning = WarehouseRules.ValidateStaffAssignment(user, hasStaffRole: true, requestedId: "abc");

        Assert.Null(warning);
    }
}
