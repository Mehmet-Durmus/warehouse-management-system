using MediatR;
using WHMS.Application.Abstractions.Persistence;

namespace WHMS.Application.Features.Queries.Catalog.GetCatalogData;

public class GetCatalogDataQueryHandler : IRequestHandler<GetCatalogDataQueryRequest, GetCatalogDataQueryResponse>
{
    private readonly ICatalogRepository _catalogRepository;

    public GetCatalogDataQueryHandler(ICatalogRepository catalogRepository)
    {
        _catalogRepository = catalogRepository;
    }

    public async Task<GetCatalogDataQueryResponse> Handle(GetCatalogDataQueryRequest request, CancellationToken cancellationToken)
    {
        var categories = await _catalogRepository.GetCatalogData();
        GetCatalogDataQueryResponse response = new() {Categories = []};

        foreach (var category in categories)
        {
            var dto = new GetCatalogDataResultCategoryDto
            {
                Id = category.Id.ToString(),
                CategoryName = category.CategoryName,
                Skus = []
            };
            response.Categories.Add(dto);

            if (category.Skus is not null)
            {
                foreach (var sku in category.Skus)
                    dto.Skus.Add(new GetCatalogDataResultSkuDto
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