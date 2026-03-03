using WHMS.Domain.Entities;

namespace WHMS.Application.Abstractions.Persistence;

public interface ICatalogRepository
{
    Task<List<Category>> GetCategories();
    Task<Category> GetCategory(Guid categoryId);
    Task AddCategory(Category category);
    void UpdateCategory(Category category);
    Task DeleteCategory(Guid categoryId);
    Task<List<SKU>> GetSkus();
    Task<SKU> GetSku(Guid skuId);
    Task<SKU> GetSkuByCategory(Guid categoryId);
    Task AddSku(SKU sku);
    void UpdateSku(SKU sku);
    Task DeleteSku(Guid skuId);
}