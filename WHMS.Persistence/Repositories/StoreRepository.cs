using WHMS.Application.Abstractions.Persistence;
using WHMS.Domain.Entities;

namespace WHMS.Persistence.Repositories;

public class StoreRepository : IStoreRepository
{
    public Task CreateStore(Store store)
    {
        throw new NotImplementedException();
    }

    public Task<Store> GetStore(Guid storeId)
    {
        throw new NotImplementedException();
    }

    public Task<List<Store>> GetStores()
    {
        throw new NotImplementedException();
    }

    public Task SoftDelete(Guid storeId)
    {
        throw new NotImplementedException();
    }

    public void Update(Store store)
    {
        throw new NotImplementedException();
    }
}