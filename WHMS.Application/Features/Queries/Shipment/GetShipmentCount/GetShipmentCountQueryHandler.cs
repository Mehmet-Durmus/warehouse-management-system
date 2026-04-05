using MediatR;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Common.Filtering.Filters;

namespace WHMS.Application.Features.Queries.Shipment.GetShipmentCount;

public class GetShipmentCountQueryHandler : IRequestHandler<GetShipmentCountQueryRequest, GetShipmentCountQueryResponse>
{
    private readonly IShipmentRepository _shipmentRepository;

    public GetShipmentCountQueryHandler(IShipmentRepository shipmentRepository)
    {
        _shipmentRepository = shipmentRepository;
    }

    public async Task<GetShipmentCountQueryResponse> Handle(GetShipmentCountQueryRequest request, CancellationToken cancellationToken)
    {
        ShipmentFilter filter = new() { IsSent = request.IsSent };
        var shipmentCount = await _shipmentRepository.GetShipmentsCount(filter);
        return new() { ShipmentCount = shipmentCount };
    }
}