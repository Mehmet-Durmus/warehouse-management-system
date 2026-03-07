namespace WHMS.Application.DTOs.Catalog;

public class CategoryDto
{
    public string? Id { get; set; }
    public string? CategoryName { get; set; }
    public List<SkuDto>? Skus { get; set; }
}