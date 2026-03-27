using Microsoft.EntityFrameworkCore;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Common.Filtering.Extensions;
using WHMS.Application.Common.Filtering.Filters;
using WHMS.Domain.Entities;
using WHMS.Persistence.Contexts;

namespace WHMS.Persistence.Repositories;

public class WarehouseRepository : IWarehouseRepository
{
    private readonly WHMSDbContext _whmsContext;

    public WarehouseRepository(WHMSDbContext whmsContext)
    {
        _whmsContext = whmsContext;
    }

    public async Task CreateWarehouse(Warehouse warehouse)
        => await _whmsContext.Warehouses.AddAsync(warehouse);

    public async Task<List<Warehouse>> GetAllWarehouses(WarehouseFilter filter)
        => await _whmsContext.Warehouses
            .Apply(filter)
            .ToListAsync();

    public async Task<Warehouse>? GetWarehouse(Guid warehouseId)
    { 
        var warehouse = await _whmsContext.Warehouses.FirstOrDefaultAsync(w => w.Id == warehouseId);
        return warehouse!;    
    }

    public async Task<int> GetWarehousesCount(WarehouseFilter filter)
        => await _whmsContext.Warehouses.AsQueryable().Apply(filter, false).CountAsync();

    public async Task SoftDelete(Guid warehouseId)
    {
        var warehouse = await GetWarehouse(warehouseId)!;
        warehouse.IsActive = false;
        UpdateWarehouse(warehouse);
    }

    public void UpdateWarehouse(Warehouse warehouse)
        => _whmsContext.Warehouses.Update(warehouse);

    public async Task<bool> WarehouseExists(Guid warehouseId)
    {
        return await _whmsContext.Warehouses.AnyAsync(w => w.Id == warehouseId);
    }

    public async Task<bool> WarehouseNameExists(string warehouseName)
        => await _whmsContext.Warehouses.AnyAsync(w => w.NormalizedName == warehouseName);
}