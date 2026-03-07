using WHMS.Application.DTOs.Catalog;

namespace WHMS.Application.Features.Queries.Catalog.GetCategory;

public class GetCategoryQueryResponse
{
    public string? Id { get; set; }
    public string? CategoryName { get; set; }
    public List<SkuDto>? Skus { get; set; }
}