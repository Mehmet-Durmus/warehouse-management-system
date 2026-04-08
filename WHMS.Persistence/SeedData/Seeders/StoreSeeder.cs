using System.Globalization;
using Microsoft.EntityFrameworkCore;
using WHMS.Application.Abstractions.Infrastructure;
using WHMS.Persistence.Contexts;
using WHMS.Persistence.SeedData.Core;
using WHMS.Persistence.SeedData.Dto;

namespace WHMS.Persistence.SeedData.Seeders;

public class StoreSeeder : ISeeder
{
    private readonly WHMSDbContext _context;
    private readonly IJsonDataLoader _jsonDataLoader;
    private readonly SeedDataResolver _seedDataResolver;

    public StoreSeeder(WHMSDbContext context, IJsonDataLoader jsonDataLoader, SeedDataResolver seedDataResolver)
    {
        _context = context;
        _jsonDataLoader = jsonDataLoader;
        _seedDataResolver = seedDataResolver;
    }
    public int Order => 3;

    public async Task SeedAsync()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "SeedData", "Data", "store.json");
        var storeData = _jsonDataLoader.Load<StoreSeedDto>(path);
        var director = await _seedDataResolver.Employee(storeData.CreatedBy);
        var random = new Random();
        DateTime date = new DateTime(2025, 02, 10).AddHours(random.Next(9, 15));

        if (!await _context.Stores.AnyAsync())
            foreach (var store in storeData.Stores)
            {
                DateTime createdAt = date.AddMinutes(random.Next(1,59));
                _context.Stores.Add(new()
                {
                    StoreName = store.StoreName,
                    NormalizedName = store.StoreName.ToUpper(CultureInfo.GetCultureInfo("tr-TR")),
                    Address = new(
                        await _seedDataResolver.CityId(store.City),
                        await _seedDataResolver.DistrictId(store.City, store.District),
                        await _seedDataResolver.NeighborhoodId(store.City, store.District, store.Neighborhood),
                        store.PostalCode,
                        store.AddressLine
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