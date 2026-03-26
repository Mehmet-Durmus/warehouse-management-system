using MediatR;
using WHMS.Application.Abstractions.Infrastructure;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.DTOs.Catalog;
using WHMS.Application.DTOs.Delivery;
using WHMS.Application.Filters;

namespace WHMS.Application.Features.Queries.Delivery.GetDeliveryItems;

public class GetDeliveryItemsQueryHandler : IRequestHandler<GetDeliveryItemsQueryRequest, GetDeliveryItemsQueryResponse>
{
    private readonly IDeliveryRepository _deliveryRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetDeliveryItemsQueryHandler(IDeliveryRepository deliveryRepository, ICurrentUserService currentUserService)
    {
        _deliveryRepository = deliveryRepository;
        _currentUserService = currentUserService;
    }

    public async Task<GetDeliveryItemsQueryResponse> Handle(GetDeliveryItemsQueryRequest request, CancellationToken cancellationToken)
    {
        var delivery = await _deliveryRepository.GetDelivery(Guid.Parse(request.DeliveryId!));

        bool isUnauthorized = delivery switch
        {
            null => true,
            not null when _currentUserService.Roles!.Contains("WarehouseManager")
                && delivery.WarehouseId != Guid.Parse(_currentUserService.WarehouseId!) => true,
            not null when _currentUserService.Roles!.Contains("WarehouseStaff")
                && delivery.WarehouseId != Guid.Parse(_currentUserService.WarehouseId!) => true,
            not null when _currentUserService.Roles!.Contains("WarehouseStaff")
                && delivery.WarehouseId == Guid.Parse(_currentUserService.WarehouseId!)
                && delivery.ReceivedAt != null => true,
            _ => false
        };

        if (isUnauthorized)
            throw new Exception("Delivery not found.");

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