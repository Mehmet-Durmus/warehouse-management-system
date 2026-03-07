using WHMS.Application.DTOs.Catalog;

namespace WHMS.Application.Features.Queries.Catalog.GetCategories;

public class GetCategoriesQueryResponse
{
    public List<CategoryDto>? Categories { get; set; }
}