using Microsoft.EntityFrameworkCore;
using WHMS.Application.Abstractions.Persistence;
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

    public async Task<List<Warehouse>> GetAllWarehouses()
        => await _whmsContext.Warehouses.ToListAsync();

    public async Task<Warehouse>? GetWarehouse(Guid warehouseId)
        => await _whmsContext.Warehouses.FirstOrDefaultAsync(w => w.Id == warehouseId);

    public async Task SoftDelete(Guid warehouseId)
    {
        Warehouse? warehouse = await GetWarehouse(warehouseId);
        warehouse.IsActive = false;
        UpdateWarehouse(warehouse);
    }

    public void UpdateWarehouse(Warehouse warehouse)
        => _whmsContext.Warehouses.Update(warehouse);
}