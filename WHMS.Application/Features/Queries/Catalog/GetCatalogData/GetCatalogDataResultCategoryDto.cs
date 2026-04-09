namespace WHMS.Application.Features.Queries.Catalog.GetCatalogData;

public class GetCatalogDataResultCategoryDto
{
    public string Id { get; set; } = null!;
    public string CategoryName { get; set; } = null!;
    public List<GetCatalogDataResultSkuDto>? Skus { get; set; }
}