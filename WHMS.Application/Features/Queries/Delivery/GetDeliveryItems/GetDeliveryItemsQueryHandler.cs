using MediatR;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.DTOs.Catalog;
using WHMS.Application.DTOs.Delivery;
using WHMS.Application.Filters;

namespace WHMS.Application.Features.Queries.Delivery.GetDeliveryItems;

public class GetDeliveryItemsQueryHandler : IRequestHandler<GetDeliveryItemsQueryRequest, GetDeliveryItemsQueryResponse>
{
    private readonly IDeliveryRepository _deliveryRepository;

    public GetDeliveryItemsQueryHandler(IDeliveryRepository deliveryRepository)
    {
        _deliveryRepository = deliveryRepository;
    }

    public async Task<GetDeliveryItemsQueryResponse> Handle(GetDeliveryItemsQueryRequest request, CancellationToken cancellationToken)
    {
        DeliveryItemFilter filter = new()
        {
            DeliveryId = request.DeliveryId,
            SkuId = request.SkuId,
            MaxQuantity = request.MaxQuantity,
            MinQuantity = request.MinQuantity,
            Page = request.Page,
            PageSize = request.PageSize
        };

        var deliveryItems = await _deliveryRepository.GetDeliveryItems(filter, true);
        int count = await _deliveryRepository.GetDeliveryItemsCount(filter);

        GetDeliveryItemsQueryResponse response = new()
        {
            Page = request.Page,
            PageSize = request.PageSize,
            TotalPage = (int)Math.Ceiling((double) count / request.PageSize),
            DeliveryItems = []
        };

        foreach (var item in deliveryItems)
            response.DeliveryItems.Add(new DeliveryItemDto
            {
                DeliveryItemId = item.Id.ToString(),
                Sku = new SkuDto
                {
                    Id = item.Sku.Id.ToString(),
                    SKUName = item.Sku.SKUName,
                    Barcode = item.Sku.Barcode,
                    UnitPrice = item.Sku.UnitPrice
                },
                Quantity = item.Quantity
            });

        return response; 
    }
}