using Microsoft.EntityFrameworkCore;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Extensions;
using WHMS.Application.Filters;
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

    public async Task<Store> GetStore(Guid storeId)
    {
        var store = await _context.Stores.FindAsync(storeId);
        return store!;
    }

    public async Task<List<Store>> GetStores(StoreFilter filter)
        => await _context.Stores.AsQueryable().ApplyStoreFilter(filter).ToListAsync();

    public async Task<int> GetStoresCount(StoreFilter filter)
     => await _context.Stores.AsQueryable().ApplyStoreFilter(filter, false).CountAsync();

    public async Task SoftDelete(Guid storeId)
    {
        var store = await GetStore(storeId);
        store.IsActive = false;
        Update(store);
    }

    public void Update(Store store)
        => _context.Stores.Update(store);
}