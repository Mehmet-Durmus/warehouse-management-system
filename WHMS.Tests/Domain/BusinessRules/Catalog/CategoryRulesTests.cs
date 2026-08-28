using WHMS.Domain.BusinessRules.Catalog;
using WHMS.Domain.Entities;
using WHMS.Domain.Exceptions;

namespace WHMS.Tests.Domain.BusinessRules.Catalog;

public class CategoryRulesTests
{
    [Fact]
    public void EnsureExists_CategoryIsNull_ThrowsNotFoundException()
    {
        var exception = Assert.Throws<NotFoundException>(() => CategoryRules.EnsureExists(null));

        Assert.Equal("Category not found.", exception.Message);
    }

    [Fact]
    public void EnsureExists_CategoryIsNotNull_ReturnsTheSameCategory()
    {
        var category = new Category { CategoryName = "market", NormalizedCategoryName = "MARKET" };

        var result = CategoryRules.EnsureExists(category);

        Assert.Same(category, result);
    }

    [Fact]
    public void EnsureNameIsUnique_NameExists_ThrowsBusinessRuleViolationException()
    {
        var exception = Assert.Throws<BusinessRuleViolationException>(() => CategoryRules.EnsureNameIsUnique(nameExists: true));

        Assert.Equal("Category already exists.", exception.Message);
    }

    [Fact]
    public void EnsureNameIsUnique_NameDoesNotExist_DoesNotThrow()
    {
        var exception = Record.Exception(() => CategoryRules.EnsureNameIsUnique(nameExists: false));

        Assert.Null(exception);
    }

    [Fact]
    public void EnsureCanBeDeleted_HasStockOrPendingDelivery_ThrowsBusinessRuleViolationException()
    {
        var exception = Assert.Throws<BusinessRuleViolationException>(() => CategoryRules.EnsureCanBeDeleted(hasStockOrPendingDelivery: true));

        Assert.Equal(
            "This category cannot be deleted because it contains SKUs that exist in stock or pending deliveries.",
            exception.Message);
    }

    [Fact]
    public void EnsureCanBeDeleted_NoStockOrPendingDelivery_DoesNotThrow()
    {
        var exception = Record.Exception(() => CategoryRules.EnsureCanBeDeleted(hasStockOrPendingDelivery: false));

        Assert.Null(exception);
    }
}
