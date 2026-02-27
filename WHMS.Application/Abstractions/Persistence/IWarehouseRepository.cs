using WHMS.Domain.Entities;

namespace WHMS.Application.Abstractions.Persistence;

public interface IWarehouseRepository
{
    Task<List<Warehouse>> GetAllWarehouses();
    Task<Warehouse> GetWarehouse(Guid warehouseId);
    Task CreateWarehouse(Warehouse warehouse);
    void UpdateWarehouse(Warehouse warehouse);
    Task SoftDelete(Guid warehouseId);
}