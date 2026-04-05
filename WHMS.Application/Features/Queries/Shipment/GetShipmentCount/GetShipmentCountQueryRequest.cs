using MediatR;

namespace WHMS.Application.Features.Queries.Shipment.GetShipmentCount;

public class GetShipmentCountQueryRequest : IRequest<GetShipmentCountQueryResponse>
{
    public bool? IsSent { get; set; }
}