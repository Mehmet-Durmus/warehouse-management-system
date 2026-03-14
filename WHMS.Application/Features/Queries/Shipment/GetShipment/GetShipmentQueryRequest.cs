using MediatR;

namespace WHMS.Application.Features.Queries.Shipment.GetShipment;

public class GetShipmentQueryRequest : IRequest<GetShipmentQueryResponse>
{
    public string? ShipmentId { get; set; }
}