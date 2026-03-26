namespace WHMS.Application.DTOs.Catalog;

public class CategoryDto
{
    public string Id { get; set; } = null!;
    public string CategoryName { get; set; } = null!;
    public List<SkuDto>? Skus { get; set; }
}