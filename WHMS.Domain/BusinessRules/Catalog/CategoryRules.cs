using WHMS.Domain.Entities;
using WHMS.Domain.Exceptions;

namespace WHMS.Domain.BusinessRules.Catalog;

public static class CategoryRules
{
    public static Category EnsureExists(Category? category)
    {
        if (category is null)
            throw new NotFoundException("Category not found.");
        return category;
    }

    public static void EnsureNameIsUnique(bool nameExists)
    {
        if (nameExists)
            throw new BusinessRuleViolationException("Category already exists.");
    }

    public static void EnsureCanBeDeleted(bool hasStockOrPendingDelivery)
    {
        if (hasStockOrPendingDelivery)
            throw new BusinessRuleViolationException(
                "This category cannot be deleted because it contains SKUs that exist in stock or pending deliveries.");
    }
}
