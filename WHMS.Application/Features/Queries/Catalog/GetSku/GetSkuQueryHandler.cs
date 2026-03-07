using MediatR;
using WHMS.Application.Abstractions.Persistence;

namespace WHMS.Application.Features.Queries.Catalog.GetSku;

public class GetSkuQueryHandler : IRequestHandler<GetSkuQueryRequest, GetSkuQueryResponse>
{
    private readonly ICatalogRepository _catalogRepository;


    public GetSkuQueryHandler(ICatalogRepository catalogRepository)
    {
        _catalogRepository = catalogRepository;
    }

    public async Task<GetSkuQueryResponse> Handle(GetSkuQueryRequest request, CancellationToken cancellationToken)
    {
        var sku = await _catalogRepository.GetSku(Guid.Parse(request.SkuId!));
        if (sku is null)
            throw new Exception("Sku is not found.");
        
        return new()
        {
            SKUName = sku.SKUName,
            Barcode = sku.Barcode,
            UnitPrice = sku.UnitPrice
        };
    }
}