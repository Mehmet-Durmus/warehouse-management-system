using WHMS.Domain.Entities;
using WHMS.Domain.Exceptions;

namespace WHMS.Domain.BusinessRules.Catalog;

public static class SkuRules
{
    public static SKU EnsureExists(SKU? sku)
    {
        if (sku is null)
            throw new NotFoundException("Sku not found.");
        return sku;
    }

    public static void EnsureNameIsUnique(bool nameExists)
    {
        if (nameExists)
            throw new BusinessRuleViolationException("Sku already exists.");
    }

    public static void EnsureCanBeDeleted(bool hasStockOrPendingDelivery)
    {
        if (hasStockOrPendingDelivery)
            throw new BusinessRuleViolationException(
                "Cannot delete SKU because it exists in stock or pending deliveries.");
    }
}
