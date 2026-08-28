using WHMS.Domain.Exceptions;

namespace WHMS.Domain.BusinessRules.Store;

public static class StoreRules
{
    public static WHMS.Domain.Entities.Store EnsureExists(WHMS.Domain.Entities.Store? store)
    {
        if (store is null)
            throw new NotFoundException("Store not found.");
        return store;
    }

    public static void EnsureNameIsUnique(bool nameExists)
    {
        if (nameExists)
            throw new BusinessRuleViolationException("This name is being used for another store.");
    }
}
