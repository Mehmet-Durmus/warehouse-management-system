using System.Security.Cryptography.X509Certificates;
using MediatR;
using WHMS.Application.Abstractions.Infrastructure;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.DTOs.Catalog;

namespace WHMS.Application.Features.Queries.Delivery.GetDelivery;

public class GetDeliveryQueryHandler : IRequestHandler<GetDeliveryQueryRequet, GetDeliveryQueryResponse>
{
    private readonly IDeliveryRepository _deliveryRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetDeliveryQueryHandler(IDeliveryRepository deliveryRepository, ICurrentUserService currentUserService)
    {
        _deliveryRepository = deliveryRepository;
        _currentUserService = currentUserService;
    }

    public async Task<GetDeliveryQueryResponse> Handle(GetDeliveryQueryRequet request, CancellationToken cancellationToken)
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


        GetDeliveryQueryResponse response = new()
        {
            DeliveryId = delivery!.Id.ToString(),
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