using WHMS.Application.Filters;
using WHMS.Domain.Entities;

namespace WHMS.Application.Abstractions.Persistence;

public interface IWarehouseRepository
{
    Task<List<Warehouse>> GetAllWarehouses(WarehouseFilter filter);
    Task<int> GetWarehousesCount(WarehouseFilter filter);
    Task<Warehouse>? GetWarehouse(Guid warehouseId);
    Task CreateWarehouse(Warehouse warehouse);
    void UpdateWarehouse(Warehouse warehouse);
    Task SoftDelete(Guid warehouseId);
    Task<bool> WarehouseExists(Guid warehouseId);
}