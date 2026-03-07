using MediatR;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.DTOs.Catalog;

namespace WHMS.Application.Features.Queries.Catalog.GetSkus;

public class GetSkusQueryHandler : IRequestHandler<GetSkusQueryRequest, GetSkusQueryResponse>
{
    private readonly ICatalogRepository _catalogRepository;

    public GetSkusQueryHandler(ICatalogRepository catalogRepository)
    {
        _catalogRepository = catalogRepository;
    }

    public async Task<GetSkusQueryResponse> Handle(GetSkusQueryRequest request, CancellationToken cancellationToken)
    {
        var skus = await _catalogRepository.GetSkus();
        GetSkusQueryResponse response = new() {Skus = []};
        foreach (var sku in skus)
            response.Skus.Add(new SkuDto
            {
                Id = sku.Id.ToString(),
                SKUName = sku.SKUName,
                Barcode = sku.Barcode,
                UnitPrice = sku.UnitPrice
            });

        return response;
    }
}