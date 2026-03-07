using MediatR;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.DTOs.Catalog;

namespace WHMS.Application.Features.Queries.Catalog.GetCategories;

public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQueryRequest, GetCategoriesQueryResponse>
{
    private readonly ICatalogRepository _catalogRepository;

    public GetCategoriesQueryHandler(ICatalogRepository catalogRepository)
    {
        _catalogRepository = catalogRepository;
    }

    public async Task<GetCategoriesQueryResponse> Handle(GetCategoriesQueryRequest request, CancellationToken cancellationToken)
    {
        var categories = await _catalogRepository.GetCategories();
        GetCategoriesQueryResponse response = new() {Categories = []};

        foreach (var category in categories)
        {
            var dto = new CategoryDto
            {
                Id = category.Id.ToString(),
                CategoryName = category.CategoryName,
                Skus = []
            };
            response.Categories.Add(dto);

            if (category.Skus is not null)
            {
                foreach (var sku in category.Skus)
                    dto.Skus.Add(new SkuDto
                    {
                        Id = sku.Id.ToString(),
                        SKUName = sku.SKUName,
                        Barcode = sku.Barcode,
                        UnitPrice = sku.UnitPrice
                    });
            }
        }
        return response;
    }
}