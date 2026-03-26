using WHMS.Application.DTOs.Catalog;

namespace WHMS.Application.Features.Queries.Catalog.GetCatalogData;

public class GetCatalogDataQueryResponse
{
    public List<CategoryDto>? Categories { get; set; }
}