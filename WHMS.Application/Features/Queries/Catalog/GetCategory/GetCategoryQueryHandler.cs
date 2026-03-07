using MediatR;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.DTOs.Catalog;

namespace WHMS.Application.Features.Queries.Catalog.GetCategory;

public class GetCategoryQueryHandler : IRequestHandler<GetCategoryQueryRequest, GetCategoryQueryResponse>
{
    private readonly ICatalogRepository _catalogRepository;

    public GetCategoryQueryHandler(ICatalogRepository catalogRepository)
    {
        _catalogRepository = catalogRepository;
    }

    public async Task<GetCategoryQueryResponse> Handle(GetCategoryQueryRequest request, CancellationToken cancellationToken)
    {
        var category = await _catalogRepository.GetCategory(Guid.Parse(request.CategoryId!));
        if (category is null)
            throw new Exception("Category not found.");

        GetCategoryQueryResponse response = new() 
        {
            Id = category.Id.ToString(),
            CategoryName = category.CategoryName,
            Skus = []
        };
        if (category.Skus is not null)
        {
            foreach (var sku in category.Skus)
                response.Skus.Add(new SkuDto
                {
                    Id = sku.Id.ToString(),
                    SKUName = sku.SKUName,
                    Barcode = sku.Barcode,
                    UnitPrice = sku.UnitPrice
                });
        }
        return response;
    }
}