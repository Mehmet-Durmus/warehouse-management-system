using Microsoft.EntityFrameworkCore;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Extensions;
using WHMS.Application.Filters;
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
    {
        var query = _whmsContext.Warehouses.AsQueryable();

        query = query
            .WhereIf(!string.IsNullOrWhiteSpace(filter.CityId), w => w.Address.CityId == Guid.Parse(filter.CityId!))
            .WhereIf(!string.IsNullOrWhiteSpace(filter.DistrictId), w => w.Address.DistrictId == Guid.Parse(filter.DistrictId!));
            
        return await query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();
    }

    public async Task<Warehouse>? GetWarehouse(Guid warehouseId)
    { 
        var warehouse = await _whmsContext.Warehouses.FirstOrDefaultAsync(w => w.Id == warehouseId);
        return warehouse!;    
    }

    public async Task<int> GetWarehousesCount(WarehouseFilter filter)
    {
        var query = _whmsContext.Warehouses.AsQueryable();

        query = query
            .WhereIf(!string.IsNullOrWhiteSpace(filter.CityId), w => w.Address.CityId == Guid.Parse(filter.CityId!))
            .WhereIf(!string.IsNullOrWhiteSpace(filter.DistrictId), w => w.Address.DistrictId == Guid.Parse(filter.DistrictId!));
        
        return await query.CountAsync();
    }

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
}