using WHMS.Domain.BusinessRules.Shared;
using WHMS.Domain.Exceptions;

namespace WHMS.Tests.Domain.BusinessRules.Shared;

public class AddressRulesTests
{
    [Fact]
    public void EnsureIsValid_AddressIsInvalid_ThrowsBusinessRuleViolationException()
    {
        var exception = Assert.Throws<BusinessRuleViolationException>(() => AddressRules.EnsureIsValid(isAddressValid: false));

        Assert.Equal("Address data is invalid.", exception.Message);
    }

    [Fact]
    public void EnsureIsValid_AddressIsValid_DoesNotThrow()
    {
        var exception = Record.Exception(() => AddressRules.EnsureIsValid(isAddressValid: true));

        Assert.Null(exception);
    }
}
