using WHMS.Domain.BusinessRules.InventoryCount;
using WHMS.Domain.Exceptions;

namespace WHMS.Tests.Domain.BusinessRules.InventoryCount;

public class InventoryCountRulesTests
{
    [Fact]
    public void EnsureAccessibleForWarehouse_InventoryCountIsNull_ThrowsNotFoundException()
    {
        var exception = Assert.Throws<NotFoundException>(() => InventoryCountRules.EnsureAccessibleForWarehouse(null, Guid.NewGuid()));

        Assert.Equal("Inventory count not found.", exception.Message);
    }

    [Fact]
    public void EnsureAccessibleForWarehouse_DifferentWarehouse_ThrowsNotFoundException()
    {
        var warehouseId = Guid.NewGuid();
        var inventoryCount = new WHMS.Domain.Entities.InventoryCount { WarehouseId = Guid.NewGuid() };

        var exception = Assert.Throws<NotFoundException>(() => InventoryCountRules.EnsureAccessibleForWarehouse(inventoryCount, warehouseId));

        Assert.Equal("Inventory count not found.", exception.Message);
    }

    [Fact]
    public void EnsureAccessibleForWarehouse_SameWarehouse_ReturnsTheSameInventoryCount()
    {
        var warehouseId = Guid.NewGuid();
        var inventoryCount = new WHMS.Domain.Entities.InventoryCount { WarehouseId = warehouseId };

        var result = InventoryCountRules.EnsureAccessibleForWarehouse(inventoryCount, warehouseId);

        Assert.Same(inventoryCount, result);
    }

    [Fact]
    public void EnsureCreationAllowed_UncompletedInventoryCountExists_ThrowsBusinessRuleViolationExceptionWithTypoPreserved()
    {
        var exception = Assert.Throws<BusinessRuleViolationException>(() => InventoryCountRules.EnsureCreationAllowed(isThereUncompletedInventoryCount: true));

        Assert.Equal("There is uncomplated inventory count.", exception.Message);
    }

    [Fact]
    public void EnsureCreationAllowed_NoUncompletedInventoryCount_DoesNotThrow()
    {
        var exception = Record.Exception(() => InventoryCountRules.EnsureCreationAllowed(isThereUncompletedInventoryCount: false));

        Assert.Null(exception);
    }

    [Fact]
    public void EnsureNotCompletedForLineCreation_AlreadyCompleted_ThrowsBusinessRuleViolationException()
    {
        var exception = Assert.Throws<BusinessRuleViolationException>(() => InventoryCountRules.EnsureNotCompletedForLineCreation(isCompleted: true));

        Assert.Equal("This inventory count already completed.", exception.Message);
    }

    [Fact]
    public void EnsureNotCompletedForLineCreation_NotCompleted_DoesNotThrow()
    {
        var exception = Record.Exception(() => InventoryCountRules.EnsureNotCompletedForLineCreation(isCompleted: false));

        Assert.Null(exception);
    }

    [Fact]
    public void EnsureNotCompleted_AlreadyCompleted_ThrowsBusinessRuleViolationException()
    {
        var exception = Assert.Throws<BusinessRuleViolationException>(() => InventoryCountRules.EnsureNotCompleted(isCompleted: true));

        Assert.Equal("Inventory count has already been completed.", exception.Message);
    }

    [Fact]
    public void EnsureNotCompleted_NotCompleted_DoesNotThrow()
    {
        var exception = Record.Exception(() => InventoryCountRules.EnsureNotCompleted(isCompleted: false));

        Assert.Null(exception);
    }

    [Fact]
    public void EnsureNotCompletedForLineUpdate_AlreadyCompleted_ThrowsWithIdInMessage()
    {
        var id = Guid.NewGuid();

        var exception = Assert.Throws<BusinessRuleViolationException>(() => InventoryCountRules.EnsureNotCompletedForLineUpdate(isCompleted: true, inventoryCountId: id));

        Assert.Equal($"{id}: Inventory count has already been completed.", exception.Message);
    }

    [Fact]
    public void EnsureNotCompletedForLineUpdate_NotCompleted_DoesNotThrow()
    {
        var exception = Record.Exception(() => InventoryCountRules.EnsureNotCompletedForLineUpdate(isCompleted: false, inventoryCountId: Guid.NewGuid()));

        Assert.Null(exception);
    }

    [Fact]
    public void EnsureLineAccessible_LineIsNull_ThrowsNotFoundException()
    {
        var exception = Assert.Throws<NotFoundException>(() => InventoryCountRules.EnsureLineAccessible(null, restrictedToOwnLines: false, currentUserId: Guid.NewGuid()));

        Assert.Equal("Inventory count line not found.", exception.Message);
    }

    [Fact]
    public void EnsureLineAccessible_RestrictedAndNotTheCreator_ThrowsNotFoundException()
    {
        var line = new WHMS.Domain.Entities.InventoryCountLine { CreatedById = Guid.NewGuid() };

        var exception = Assert.Throws<NotFoundException>(() => InventoryCountRules.EnsureLineAccessible(line, restrictedToOwnLines: true, currentUserId: Guid.NewGuid()));

        Assert.Equal("Inventory count line not found.", exception.Message);
    }

    [Fact]
    public void EnsureLineAccessible_RestrictedAndIsTheCreator_ReturnsTheSameLine()
    {
        var userId = Guid.NewGuid();
        var line = new WHMS.Domain.Entities.InventoryCountLine { CreatedById = userId };

        var result = InventoryCountRules.EnsureLineAccessible(line, restrictedToOwnLines: true, currentUserId: userId);

        Assert.Same(line, result);
    }

    [Fact]
    public void EnsureLineAccessible_NotRestricted_ReturnsTheSameLineRegardlessOfCreator()
    {
        var line = new WHMS.Domain.Entities.InventoryCountLine { CreatedById = Guid.NewGuid() };

        var result = InventoryCountRules.EnsureLineAccessible(line, restrictedToOwnLines: false, currentUserId: Guid.NewGuid());

        Assert.Same(line, result);
    }

    [Theory]
    [InlineData(12, 15, -3)]
    [InlineData(10, 10, 0)]
    [InlineData(20, 8, 12)]
    public void CalculateVariance_ReturnsQuantityMinusCurrentStock(int quantity, int currentStock, int expected)
    {
        var result = InventoryCountRules.CalculateVariance(quantity, currentStock);

        Assert.Equal(expected, result);
    }
}
