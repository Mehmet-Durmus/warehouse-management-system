using Microsoft.EntityFrameworkCore;
using WHMS.Application.Abstractions.Infrastructure;
using WHMS.Persistence.Contexts;
using WHMS.Persistence.SeedData.Core;
using WHMS.Persistence.SeedData.Dto;

namespace WHMS.Persistence.SeedData.Seeders;

public class StockStateSeeder : ISeeder
{
    private readonly WHMSDbContext _context;
    private readonly IJsonDataLoader _jsonDataLoader;
    private readonly SeedDataResolver _seedDataResolver;

    public StockStateSeeder(WHMSDbContext context, IJsonDataLoader jsonDataLoader, SeedDataResolver seedDataResolver)
    {
        _context = context;
        _jsonDataLoader = jsonDataLoader;
        _seedDataResolver = seedDataResolver;
    }
    public int Order => 9;

    public async Task SeedAsync()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "SeedData", "Data", "stockStates.json");
        var stockStateData = _jsonDataLoader.Load<StockStatesSeedDto>(path);

        if (!await _context.StockStates.AnyAsync())
            foreach (var warehouse in stockStateData.StockStates)
                foreach (var data in warehouse.Data)
                    _context.StockStates.Add(new()
                    {
                        WarehouseId = await _seedDataResolver.WarehouseId(warehouse.Warehouse),
                        SkuId = await _seedDataResolver.SkuId(data.Sku),
                        Quantity = data.Quantity
                    });
        
        await _context.SaveChangesAsync();
    }
}