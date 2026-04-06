using MediatR;
using WHMS.Application.Abstractions.Infrastructure;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Common.Constants;
using WHMS.Application.Common.Filtering.Filters;
using WHMS.Application.DTOs.Catalog;
using WHMS.Application.DTOs.Delivery;

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

        bool notFound = delivery switch
        {
            null => true,
            not null when _currentUserService.Roles!.Contains(ApplicationRole.WarehouseManager)
                && delivery.WarehouseId != Guid.Parse(_currentUserService.WarehouseId!) => true,
            not null when _currentUserService.Roles!.Contains(ApplicationRole.WarehouseStaff)
                && delivery.WarehouseId != Guid.Parse(_currentUserService.WarehouseId!) => true,
            not null when _currentUserService.Roles!.Contains(ApplicationRole.WarehouseStaff)
                && delivery.WarehouseId == Guid.Parse(_currentUserService.WarehouseId!)
                && delivery.ReceivedAt != null => true,
            _ => false
        };

        if (notFound)
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

        GetDeliveryItemsQueryResponse response = new() { DeliveryItems = [] };
        response.Pagination = new(
            request.Page,
            request.PageSize,
            (int)Math.Ceiling((double) count / request.PageSize)
        );

        foreach (var item in deliveryItems)
            response.DeliveryItems.Add(new()
            {
                DeliveryItemId = item.Id.ToString(),
                Sku = new()
                {
                    SkuId = item.Sku.Id.ToString(),
                    SkuName = item.Sku.SKUName,
                    Barcode = item.Sku.Barcode,
                    UnitPrice = item.Sku.UnitPrice
                },
                Quantity = item.Quantity
            });

        return response; 
    }
}