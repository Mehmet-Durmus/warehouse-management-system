using WHMS.Domain.Exceptions;

namespace WHMS.Domain.BusinessRules.Shared;

public static class AddressRules
{
    public static void EnsureIsValid(bool isAddressValid)
    {
        if (!isAddressValid)
            throw new BusinessRuleViolationException("Address data is invalid.");
    }
}
