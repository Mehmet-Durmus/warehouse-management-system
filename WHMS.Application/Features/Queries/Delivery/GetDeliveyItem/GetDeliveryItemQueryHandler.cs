using MediatR;
using WHMS.Application.Abstractions.Persistence;

namespace WHMS.Application.Features.Queries.Delivery.GetDeliveryItem;

public class GetDeliveryItemQueryHandler : IRequestHandler<GetDeliveryItemQueryRequest, GetDeliveryItemQueryResponse>
{
    private readonly IDeliveryRepository _deliveryRepository;

    public GetDeliveryItemQueryHandler(IDeliveryRepository deliveryRepository)
    {
        _deliveryRepository = deliveryRepository;
    }

    public async Task<GetDeliveryItemQueryResponse> Handle(GetDeliveryItemQueryRequest request, CancellationToken cancellationToken)
    {
        var delivery = await _deliveryRepository.GetDeliveryItem(Guid.Parse(request.DeliveryItemId!));
        if (delivery is null)
            throw new Exception("Delivery item not found");

        return new()
        {
            DeliveryId = delivery.DeliveryId.ToString(),
            SkuId = delivery.SkuId.ToString(),
            SKUName = delivery.Sku.SKUName,
            Quantity = delivery.Quantity,
            CreatedAt = delivery.CreatedAt,
            CreatedById = delivery.CreatedById,
            CreatedByName = delivery.CreatedByName,
            CreatedByUserName = delivery.CreatedByUserName,
            UpdatedAt = delivery.UpdatedAt,
            UpdatedById = delivery.UpdatedById,
            UpdatedByName = delivery.UpdatedByName,
            UpdatedByUserName = delivery.UpdatedByUserName,
        };
    }
}