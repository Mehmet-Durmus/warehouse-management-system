using MediatR;
using WHMS.Application.Abstractions.Infrastructure;
using WHMS.Application.Abstractions.Persistence;

namespace WHMS.Application.Features.Queries.Delivery.GetDeliveryItem;

public class GetDeliveryItemQueryHandler : IRequestHandler<GetDeliveryItemQueryRequest, GetDeliveryItemQueryResponse>
{
    private readonly IDeliveryRepository _deliveryRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetDeliveryItemQueryHandler(IDeliveryRepository deliveryRepository, ICurrentUserService currentUserService)
    {
        _deliveryRepository = deliveryRepository;
        _currentUserService = currentUserService;
    }

    public async Task<GetDeliveryItemQueryResponse> Handle(GetDeliveryItemQueryRequest request, CancellationToken cancellationToken)
    {
        var deliveryItem = await _deliveryRepository.GetDeliveryItem(Guid.Parse(request.DeliveryItemId!));
        
        bool notFound = deliveryItem switch
        {
            null => true,
            not null when _currentUserService.Roles!.Contains("WarehouseManager")
                && deliveryItem.Delivery.WarehouseId != Guid.Parse(_currentUserService.WarehouseId!) => true,
            not null when _currentUserService.Roles!.Contains("WarehouseStaff")
                && deliveryItem.Delivery.WarehouseId != Guid.Parse(_currentUserService.WarehouseId!) => true,
            not null when _currentUserService.Roles!.Contains("WarehouseStaff")
                && deliveryItem.Delivery.WarehouseId == Guid.Parse(_currentUserService.WarehouseId!)
                && deliveryItem.Delivery.ReceivedAt != null => true,
            _ => false
        };

        if (notFound)
            throw new Exception("Delivery item not found");

        return new()
        {
            DeliveryId = deliveryItem!.DeliveryId.ToString(),
            Sku = new()
            {
                SkuId = deliveryItem.SkuId.ToString(),
                SkuName = deliveryItem.Sku.SKUName,
                Barcode = deliveryItem.Sku.Barcode,
                UnitPrice = deliveryItem.Sku.UnitPrice,
            },
            Quantity = deliveryItem.Quantity
        };
    }
}