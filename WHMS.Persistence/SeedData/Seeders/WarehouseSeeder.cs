using System.Globalization;
using Microsoft.EntityFrameworkCore;
using WHMS.Application.Abstractions.Infrastructure;
using WHMS.Persistence.Contexts;
using WHMS.Persistence.SeedData.Core;
using WHMS.Persistence.SeedData.Dto;

namespace WHMS.Persistence.SeedData.Seeders;

public class WarehouseSeeder : ISeeder
{
    private readonly WHMSDbContext _context;
    private readonly IJsonDataLoader _jsonDataLoader;
    private readonly SeedDataResolver _seedDataResolver;

    public WarehouseSeeder(WHMSDbContext context, IJsonDataLoader jsonDataLoader, SeedDataResolver seedDataResolver)
    {
        _context = context;
        _jsonDataLoader = jsonDataLoader;
        _seedDataResolver = seedDataResolver;
    }
    public int Order => 2;

    public async Task SeedAsync()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "SeedData", "Data", "warehouse.json");
        var warehouseData = _jsonDataLoader.Load<WarehouseSeedDto>(path);
        var director = await _seedDataResolver.Employee(warehouseData.CreatedBy);
        var random = new Random();
        DateTime date = new DateTime(2025, 02, 10).AddHours(random.Next(9, 15));

        if (!await _context.Warehouses.AnyAsync())
            foreach (var warehouse in warehouseData.Warehouses)
            {
                DateTime createdAt = date.AddMinutes(random.Next(1,59));
                _context.Warehouses.Add(new()
                {
                    WarehouseName = warehouse.WarehouseName,
                    NormalizedName = warehouse.WarehouseName.ToUpper(CultureInfo.GetCultureInfo("tr-TR")),
                    Address = new(
                        await _seedDataResolver.CityId(warehouse.City),
                        await _seedDataResolver.DistrictId(warehouse.City, warehouse.District),
                        await _seedDataResolver.NeighborhoodId(warehouse.City, warehouse.District, warehouse.Neighborhood),
                        warehouse.PostalCode,
                        warehouse.AddressLine
                    ),
                    CreatedAt = createdAt.AddSeconds(random.Next(0,59)),
                    CreatedById = director!.Id,
                    CreatedByName = director.FullName,
                    CreatedByUserName = director.UserName,
                    UpdatedAt = createdAt.AddSeconds(random.Next(0,59)),
                    UpdatedById = director!.Id,
                    UpdatedByName = director.FullName,
                    UpdatedByUserName = director.UserName
                });
            }

        await _context.SaveChangesAsync();
    }
}