using MediatR;
using WHMS.Application.Abstractions.Persistence;

namespace WHMS.Application.Features.Queries.Shipment.GetShipmentItem;

public class GetShipmentItemQueryHandler : IRequestHandler<GetShipmentItemQueryRequest, GetShipmentItemQueryResponse>
{
    private readonly IShipmentRepository _shipmentRepository;

    public GetShipmentItemQueryHandler(IShipmentRepository shipmentRepository)
    {
        _shipmentRepository = shipmentRepository;
    }

    public async Task<GetShipmentItemQueryResponse> Handle(GetShipmentItemQueryRequest request, CancellationToken cancellationToken)
    {
        var shipmentItem = await _shipmentRepository.GetShipmentItem(Guid.Parse(request.ShipmentItemId!));
        if (shipmentItem is null)
            throw new Exception("Shipment item not found.");
        
        return new()
        {
            ShipmentId = shipmentItem.ShipmentId.ToString(),
            SkuId = shipmentItem.SkuId.ToString(),
            SKUName = shipmentItem.SKU!.SKUName,
            Quantity = shipmentItem.Quantity,
            CreatedAt = shipmentItem.CreatedAt,
            CreatedById = shipmentItem.CreatedById,
            CreatedByName = shipmentItem.CreatedByName,
            CreatedByUserName = shipmentItem.CreatedByUserName,
            UpdatedAt = shipmentItem.UpdatedAt,
            UpdatedById = shipmentItem.UpdatedById,
            UpdatedByName = shipmentItem.UpdatedByName,
            UpdatedByUserName = shipmentItem.UpdatedByUserName
        };
    }
}