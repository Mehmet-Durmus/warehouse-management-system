using WHMS.Domain.BusinessRules.Catalog;
using WHMS.Domain.Entities;
using WHMS.Domain.Exceptions;

namespace WHMS.Tests.Domain.BusinessRules.Catalog;

public class SkuRulesTests
{
    [Fact]
    public void EnsureExists_SkuIsNull_ThrowsNotFoundException()
    {
        var exception = Assert.Throws<NotFoundException>(() => SkuRules.EnsureExists(null));

        Assert.Equal("Sku not found.", exception.Message);
    }

    [Fact]
    public void EnsureExists_SkuIsNotNull_ReturnsTheSameSku()
    {
        var sku = new SKU { SKUName = "kola", NormalizedSKUName = "KOLA", Barcode = "8690000000001" };

        var result = SkuRules.EnsureExists(sku);

        Assert.Same(sku, result);
    }

    [Fact]
    public void EnsureNameIsUnique_NameExists_ThrowsBusinessRuleViolationException()
    {
        var exception = Assert.Throws<BusinessRuleViolationException>(() => SkuRules.EnsureNameIsUnique(nameExists: true));

        Assert.Equal("Sku already exists.", exception.Message);
    }

    [Fact]
    public void EnsureNameIsUnique_NameDoesNotExist_DoesNotThrow()
    {
        var exception = Record.Exception(() => SkuRules.EnsureNameIsUnique(nameExists: false));

        Assert.Null(exception);
    }

    [Fact]
    public void EnsureCanBeDeleted_HasStockOrPendingDelivery_ThrowsBusinessRuleViolationException()
    {
        var exception = Assert.Throws<BusinessRuleViolationException>(() => SkuRules.EnsureCanBeDeleted(hasStockOrPendingDelivery: true));

        Assert.Equal("Cannot delete SKU because it exists in stock or pending deliveries.", exception.Message);
    }

    [Fact]
    public void EnsureCanBeDeleted_NoStockOrPendingDelivery_DoesNotThrow()
    {
        var exception = Record.Exception(() => SkuRules.EnsureCanBeDeleted(hasStockOrPendingDelivery: false));

        Assert.Null(exception);
    }
}
