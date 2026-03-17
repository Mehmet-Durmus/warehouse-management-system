using MediatR;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.DTOs.Catalog;

namespace WHMS.Application.Features.Queries.Delivery.GetDelivery;

public class GetDeliveryQueryHandler : IRequestHandler<GetDeliveryQueryRequet, GetDeliveryQueryResponse>
{
    private readonly IDeliveryRepository _deliveryRepository;

    public GetDeliveryQueryHandler(IDeliveryRepository deliveryRepository)
    {
        _deliveryRepository = deliveryRepository;
    }

    public async Task<GetDeliveryQueryResponse> Handle(GetDeliveryQueryRequet request, CancellationToken cancellationToken)
    {
        var delivery = await _deliveryRepository.GetDelivery(Guid.Parse(request.DeliveryId!));

        GetDeliveryQueryResponse response = new()
        {
            DeliveryId = delivery.Id.ToString(),
            WarehouseId = delivery.WarehouseId.ToString(),
            DeliveryItems = [],
            ReceivedAt = delivery.ReceivedAt,
            ReceivedById = delivery.ReceivedById.ToString(),
            CreatedAt = delivery.CreatedAt,
            CreatedById = delivery.CreatedById.ToString(),
            CreatedByName = delivery.CreatedByName!,
           CreatedByUserName = delivery.CreatedByUserName!,
            UpdatedAt = delivery.UpdatedAt,
            UpdatedById = delivery.UpdatedById.ToString(),
            UpdatedByName = delivery.UpdatedByName!,
            UpdatedByUserName = delivery.UpdatedByUserName!,
        };

        foreach (var item in delivery.DeliveryItems!)
            response.DeliveryItems.Add( new DTOs.Delivery.DeliveryItemDto
            {
                DeliveryItemId = item.Id.ToString(),
                Sku = new SkuDto
                {
                    Id = item.Sku.Id.ToString(),
                    SKUName = item.Sku.SKUName,
                    Barcode = item.Sku.Barcode,
                    UnitPrice = item.Sku.UnitPrice,
                },
                Quantity = item.Quantity
            });
        return response;
    }
}