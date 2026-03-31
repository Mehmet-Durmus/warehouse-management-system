using MediatR;
using WHMS.Application.Abstractions.Infrastructure;
using WHMS.Application.Abstractions.Persistence;

namespace WHMS.Application.Features.Queries.Shipment.GetShipmentItem;

public class GetShipmentItemQueryHandler : IRequestHandler<GetShipmentItemQueryRequest, GetShipmentItemQueryResponse>
{
    private readonly IShipmentRepository _shipmentRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetShipmentItemQueryHandler(IShipmentRepository shipmentRepository, ICurrentUserService currentUserService)
    {
        _shipmentRepository = shipmentRepository;
        _currentUserService = currentUserService;
    }

    public async Task<GetShipmentItemQueryResponse> Handle(GetShipmentItemQueryRequest request, CancellationToken cancellationToken)
    {
        var shipmentItem = await _shipmentRepository.GetShipmentItem(Guid.Parse(request.ShipmentItemId!));
        bool notFound = shipmentItem switch
        {
            null => true,
            not null when _currentUserService.Roles!.Contains("WarehouseManager")
                && shipmentItem.Shipment!.WarehouseId != Guid.Parse(_currentUserService.WarehouseId!) => true,
            not null when _currentUserService.Roles!.Contains("WarehouseStaff")
                && shipmentItem.Shipment!.WarehouseId != Guid.Parse(_currentUserService.WarehouseId!) => true,
            not null when _currentUserService.Roles!.Contains("WarehouseStaff")
                && shipmentItem.Shipment!.WarehouseId == Guid.Parse(_currentUserService.WarehouseId!)
                && shipmentItem.Shipment!.SendingDate != null => true,
            _ => false
        };
        if (notFound)
            throw new Exception("Shipment item not found.");
        
        return new()
        {
            ShimpentId = shipmentItem!.ShipmentId.ToString(),
            Sku = new()
            {
                SkuId = shipmentItem.SkuId.ToString(),
                SkuName = shipmentItem.SKU!.SKUName,
                Barcode = shipmentItem.SKU!.Barcode,
                UnitPrice = shipmentItem.SKU!.UnitPrice,
            },
            Quantity = shipmentItem.Quantity
        };
    }
}