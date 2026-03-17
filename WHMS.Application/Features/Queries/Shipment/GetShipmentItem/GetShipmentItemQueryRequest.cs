using MediatR;

namespace WHMS.Application.Features.Queries.Shipment.GetShipmentItem;

public class GetShipmentItemQueryRequest : IRequest<GetShipmentItemQueryResponse>
{
    public string? ShipmentItemId { get; set; }
}