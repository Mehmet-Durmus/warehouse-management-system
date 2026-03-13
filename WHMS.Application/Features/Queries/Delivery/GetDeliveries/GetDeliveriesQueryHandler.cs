using MediatR;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Filters;

namespace WHMS.Application.Features.Queries.Delivery.GetDeliveries;

public class GetDeliveriesQueryHandler : IRequestHandler<GetDeliveriesQueryRequest, GetDeliveriesQueryResponse>
{
    private readonly IDeliveryRepository _deliveryRepository;

    public GetDeliveriesQueryHandler(IDeliveryRepository deliveryRepository)
    {
        _deliveryRepository = deliveryRepository;
    }

    public async Task<GetDeliveriesQueryResponse> Handle(GetDeliveriesQueryRequest request, CancellationToken cancellationToken)
    {
        DeliveryFilter filter = new()
        {
            WarehouseId = request.WarehouseId,
            CityId = request.CityId,
            DistrictId = request.DistrictId,
            IsReceived = request.IsReceived,
            Page = request.Page,
            PageSize = request.PageSize
        };
        var deliveries = await _deliveryRepository.GetDeliveries(filter, true);
        int count = await _deliveryRepository.GetDeliveriesCount(filter);
        GetDeliveriesQueryResponse response = new()
        {
            Page = request.Page,
            PageSize = request.PageSize,
            TotalPage = (int)Math.Ceiling((double)count / request.PageSize),
            Deliveries = [] 
        };

        foreach (var delivery in deliveries)
        {
            var deliveryDto = new DTOs.Delivery.DeliveryDto
            {
                DeliveryId = delivery.Id.ToString(),
                WarehouseId = delivery.WarehouseId.ToString(),
                DeliveryItems = [],
                ReceivedAt = delivery.ReceivedAt,
                ReceivedById = delivery.ReceivedById.ToString(),
                CreatedAt = delivery.CreatedAt,
                CreatedById = delivery.CreatedById.ToString()!,
                CreatedByName = delivery.CreatedByName,
                CreatedByUserName = delivery.CreatedByUserName,
                UpdatedAt = delivery.UpdatedAt,
                UpdatedById = delivery.UpdatedById.ToString(),
                UpdatedByName = delivery.UpdatedByName,
                UpdatedByUserName = delivery.UpdatedByUserName
            };
            response.Deliveries.Add(deliveryDto);

            if (delivery.DeliveryItems is not null)
            {
                foreach (var item in delivery.DeliveryItems)
                {
                    deliveryDto.DeliveryItems.Add(new DTOs.Delivery.DeliveryItemDto
                    {
                        DeliveryItemId = item.Id.ToString(),
                        Sku = new DTOs.Catalog.SkuDto
                        {
                            Id = item.SkuId.ToString(),
                            SKUName = item.Sku.SKUName,
                            Barcode = item.Sku.Barcode,
                            UnitPrice = item.Sku.UnitPrice
                        },
                        Quantity = item.Quantity
                    });
                }
            }
        }
        return response;
    }
}