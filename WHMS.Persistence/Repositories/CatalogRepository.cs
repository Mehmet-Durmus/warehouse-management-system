using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Domain.Entities;
using WHMS.Persistence.Contexts;

namespace WHMS.Persistence.Repositories;

public class CatalogRepository : ICatalogRepository
{
    private readonly WHMSDbContext _context;

    public CatalogRepository(WHMSDbContext context)
    {
        _context = context;
    }

    public async Task AddCategory(Category category)
        => await _context.Categories.AddAsync(category);

    public async Task AddSku(SKU sku)
        => await _context.SKUs.AddAsync(sku);

    public async Task DeleteCategory(Guid categoryId)
    {
        var category = await GetCategory(categoryId);
        category.IsActive = false;
        UpdateCategory(category);
    }

    public async Task DeleteSku(Guid skuId)
    {
        var sku = await GetSku(skuId);
        sku.IsActive = false;
        UpdateSku(sku);
    }

    public async Task<List<Category>> GetCategories()
        => await _context.Categories.ToListAsync();

    public async Task<Category> GetCategory(Guid categoryId)
    {
        var category = await _context.Categories.FindAsync(categoryId);
        return category!;
    }

    public async Task<SKU> GetSku(Guid skuId)
    {
        var sku = await _context.SKUs.FindAsync(skuId);
        return sku!;
    }

    public async Task<List<SKU>> GetSkusByCategory(Guid categoryId)
        => await _context.SKUs.Where(s => s.CategoryId == categoryId).ToListAsync();

    public async Task<List<SKU>> GetSkus()
        => await _context.SKUs.ToListAsync();

    public void UpdateCategory(Category category)
        => _context.Update(category);

    public void UpdateSku(SKU sku)
        => _context.Update(sku);

    public async Task<bool> IsCategoryNameExists(string normalizedCategoryName)
        => await _context.Categories.AnyAsync(c => c.NormalizedCategoryName == normalizedCategoryName);
}