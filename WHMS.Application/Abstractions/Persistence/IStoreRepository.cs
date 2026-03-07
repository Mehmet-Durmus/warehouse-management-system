using WHMS.Domain.Entities;

namespace WHMS.Application.Abstractions.Persistence;

public interface IStoreRepository
{
    Task<List<Store>> GetStores();
    Task<Store> GetStore(Guid storeId);
    Task CreateStore(Store store);
    void Update(Store store);
    Task SoftDelete(Guid storeId);
}