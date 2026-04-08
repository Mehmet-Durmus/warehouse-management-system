using Microsoft.EntityFrameworkCore;
using WHMS.Application.Abstractions.Infrastructure;
using WHMS.Domain.Entities;
using WHMS.Persistence.Contexts;
using WHMS.Persistence.SeedData.Core;
using WHMS.Persistence.SeedData.Dto;

namespace WHMS.Persistence.SeedData.Seeders;

public class WasteRecordSeeder : ISeeder
{
    private readonly WHMSDbContext _context;
    private readonly IJsonDataLoader _jsonDataLoader;
    private readonly SeedDataResolver _seedDataResolver;

    public WasteRecordSeeder(WHMSDbContext context, IJsonDataLoader jsonDataLoader, SeedDataResolver seedDataResolver)
    {
        _context = context;
        _jsonDataLoader = jsonDataLoader;
        _seedDataResolver = seedDataResolver;
    }
    public int Order => 7;

    public async Task SeedAsync()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "SeedData", "Data", "wasteRecords.json");
        var wasteRecordData = _jsonDataLoader.Load<WasteRecordSeedDto>(path);

        if (!await _context.WasteRecords.AnyAsync())
            foreach (var wasteRecord in wasteRecordData.WasteRecords)
            {
                var createdBy = await _seedDataResolver.Employee(wasteRecord.CreatedBy);
                _context.WasteRecords.Add(new()
                {
                    WarehouseId = await _seedDataResolver.WarehouseId(wasteRecord.Warehouse),
                    SkuId = await _seedDataResolver.SkuId(wasteRecord.Sku),
                    Quantity = wasteRecord.Quantity,
                    Description = wasteRecord.Description,
                    CreatedAt = wasteRecord.CreatedAt,
                    CreatedById = createdBy!.Id,
                    CreatedByName = createdBy.FullName,
                    CreatedByUserName = createdBy.UserName,
                    UpdatedAt = wasteRecord.CreatedAt,
                    UpdatedById = createdBy!.Id,
                    UpdatedByName = createdBy.FullName,
                    UpdatedByUserName = createdBy.UserName
                });
            }
        
        await _context.SaveChangesAsync();
    }
}