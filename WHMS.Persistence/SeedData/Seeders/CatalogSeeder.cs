using System.Globalization;
using Microsoft.EntityFrameworkCore;
using WHMS.Application.Abstractions.Infrastructure;
using WHMS.Persistence.Contexts;
using WHMS.Persistence.SeedData.Core;
using WHMS.Persistence.SeedData.Dto;

namespace WHMS.Persistence.SeedData.Seeders;

public class CatalogSeeder : ISeeder
{
    private readonly WHMSDbContext _context;
    private readonly IJsonDataLoader _jsonDataLoader;
    private readonly SeedDataResolver _seedDataResolver;

    public CatalogSeeder(WHMSDbContext context, IJsonDataLoader jsonDataLoader, SeedDataResolver seedDataResolver)
    {
        _context = context;
        _jsonDataLoader = jsonDataLoader;
        _seedDataResolver = seedDataResolver;
    }

    public int Order => 1;

    public async Task SeedAsync()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "SeedData", "Data", "catalog.json");
        var catalogData = _jsonDataLoader.Load<CatalogSeedDto>(path);
        var director = await _seedDataResolver.Employee(catalogData.CreatedBy);
        var random = new Random();
        DateTime createdAt = new DateTime(2025, 02, 10).AddHours(random.Next(9, 15));

        if (!await _context.Categories.AnyAsync())
            foreach(var category in catalogData.Categories)
            {
                Domain.Entities.Category newCategory = new()
                {
                    CategoryName = category.CategoryName,
                    NormalizedCategoryName = category.CategoryName.ToUpper(CultureInfo.GetCultureInfo("tr-TR")),
                    CreatedAt = createdAt.AddSeconds(random.Next(0,59)),
                    CreatedById = director!.Id,
                    CreatedByName = director.FullName,
                    CreatedByUserName = director.UserName,
                    UpdatedAt = createdAt.AddSeconds(random.Next(0,59)),
                    UpdatedById = director!.Id,
                    UpdatedByName = director.FullName,
                    UpdatedByUserName = director.UserName
                };
                _context.Categories.Add(newCategory);

                foreach (var sku in category.Skus)
                    _context.SKUs.Add(new Domain.Entities.SKU
                    {
                        SKUName = sku.SKUName,
                        NormalizedSKUName = sku.SKUName.ToUpper(CultureInfo.GetCultureInfo("tr-TR")),
                        Barcode = sku.Barcode,
                        UnitPrice = sku.UnitPrice,
                        CategoryId =newCategory.Id,
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