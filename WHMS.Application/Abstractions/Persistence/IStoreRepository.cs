using WHMS.Application.Common.Filtering.Filters;
using WHMS.Domain.Entities;

namespace WHMS.Application.Abstractions.Persistence;

public interface IStoreRepository
{
    Task<List<Store>> GetStores(StoreFilter filter, bool applyPagination);
    Task<int> GetStoresCount(StoreFilter filter);
    Task<Store> GetStore(Guid storeId);
    Task CreateStore(Store store);
    void Update(Store store);
    Task SoftDelete(Guid storeId);
}