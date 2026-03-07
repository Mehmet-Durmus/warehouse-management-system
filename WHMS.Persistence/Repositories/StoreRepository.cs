using WHMS.Application.Abstractions.Persistence;
using WHMS.Domain.Entities;
using WHMS.Persistence.Contexts;

namespace WHMS.Persistence.Repositories;

public class StoreRepository : IStoreRepository
{
    private readonly WHMSDbContext _context;

    public StoreRepository(WHMSDbContext context)
    {
        _context = context;
    }

    public async Task CreateStore(Store store)
        => await _context.Stores.AddAsync(store);

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