using WHMS.Domain.BusinessRules.Store;
using WHMS.Domain.Exceptions;
using WHMS.Domain.ValueObjects;

namespace WHMS.Tests.Domain.BusinessRules.Store;

public class StoreRulesTests
{
    [Fact]
    public void EnsureExists_StoreIsNull_ThrowsNotFoundException()
    {
        var exception = Assert.Throws<NotFoundException>(() => StoreRules.EnsureExists(null));

        Assert.Equal("Store not found.", exception.Message);
    }

    [Fact]
    public void EnsureExists_StoreIsNotNull_ReturnsTheSameStore()
    {
        var store = new WHMS.Domain.Entities.Store
        {
            StoreName = "market",
            NormalizedName = "MARKET",
            Address = new Address(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "34000", "Cadde")
        };

        var result = StoreRules.EnsureExists(store);

        Assert.Same(store, result);
    }

    [Fact]
    public void EnsureNameIsUnique_NameExists_ThrowsBusinessRuleViolationException()
    {
        var exception = Assert.Throws<BusinessRuleViolationException>(() => StoreRules.EnsureNameIsUnique(nameExists: true));

        Assert.Equal("This name is being used for another store.", exception.Message);
    }

    [Fact]
    public void EnsureNameIsUnique_NameDoesNotExist_DoesNotThrow()
    {
        var exception = Record.Exception(() => StoreRules.EnsureNameIsUnique(nameExists: false));

        Assert.Null(exception);
    }
}
