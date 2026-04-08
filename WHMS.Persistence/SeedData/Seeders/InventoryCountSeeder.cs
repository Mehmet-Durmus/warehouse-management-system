using Microsoft.EntityFrameworkCore;
using WHMS.Application.Abstractions.Infrastructure;
using WHMS.Domain.Entities;
using WHMS.Persistence.Contexts;
using WHMS.Persistence.SeedData.Core;
using WHMS.Persistence.SeedData.Dto;

namespace WHMS.Persistence.SeedData.Seeders;

public class InventoryCountSeeder : ISeeder
{
    private readonly WHMSDbContext _context;
    private readonly IJsonDataLoader _jsonDataLoader;
    private readonly SeedDataResolver _seedDataResolver;

    public InventoryCountSeeder(WHMSDbContext context, IJsonDataLoader jsonDataLoader, SeedDataResolver seedDataResolver)
    {
        _context = context;
        _jsonDataLoader = jsonDataLoader;
        _seedDataResolver = seedDataResolver;
    }
    public int Order => 8;

    public async Task SeedAsync()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "SeedData", "Data", "inventoryCounts.json");
        var inventoryCounts = _jsonDataLoader.Load<InventoryCountsSeedDto>(path);
        Random random = new();

        if (!await _context.InventoryCounts.AnyAsync())
        {
            foreach (var count in inventoryCounts.Counts)
            {
                var manager = await _seedDataResolver.GetManagerByWarehouse(count.Warehouse);
                InventoryCount newCount = new()
                {
                    WarehouseId = await _seedDataResolver.WarehouseId(count.Warehouse),
                    IsCompleted = count.IsCompleted,
                    CreatedAt = count.CreatedAt,
                    CreatedById = manager!.Id,
                    CreatedByName = manager.FullName,
                    CreatedByUserName = manager.UserName,
                    UpdatedAt = count.CreatedAt,
                    UpdatedById = manager!.Id,
                    UpdatedByName = manager.FullName,
                    UpdatedByUserName = manager.UserName
                };
                _context.InventoryCounts.Add(newCount);

                foreach (var countLine in count.CountLines)
                {
                    var createdBy = await _seedDataResolver.Employee(countLine.CreatedBy);
                    DateTime createdAt = count.CreatedAt.AddHours(random.Next(1, 4));
                    InventoryCountLine newCountLine = new()
                    {
                        InventoryCountId = newCount.Id,
                        SkuId = await _seedDataResolver.SkuId(countLine.Sku),
                        Quantity = countLine.Quantity,
                        Variance = countLine.Variance,
                        CreatedAt = createdAt,
                        CreatedById = createdBy!.Id,
                        CreatedByName = createdBy.FullName,
                        CreatedByUserName = createdBy.UserName,
                        UpdatedAt = createdAt,
                        UpdatedById = createdBy!.Id,
                        UpdatedByName = createdBy.FullName,
                        UpdatedByUserName = createdBy.UserName,
                    };
                    // newCountLine.InventoryCountId = newCount.Id;
                    // newCountLine.SkuId = await _seedDataResolver.SkuId(countLine.Sku);
                    // newCountLine.Quantity = countLine.Quantity;
                    // newCountLine.Variance = countLine.Variance;
                    // newCountLine.CreatedAt = createdAt;
                    // newCountLine.CreatedById = createdBy!.Id;
                    // newCountLine.CreatedByName = createdBy.FullName;
                    // newCountLine.CreatedByUserName = createdBy.UserName;
                    // newCountLine.UpdatedAt = createdAt;
                    // newCountLine.UpdatedById = createdBy!.Id;
                    // newCountLine.UpdatedByName = createdBy.FullName;
                    // newCountLine.UpdatedByUserName = createdBy.UserName;
                    _context.InventoryCountLines.Add(newCountLine);
                }
            }
        }

        await _context.SaveChangesAsync();
    }
}