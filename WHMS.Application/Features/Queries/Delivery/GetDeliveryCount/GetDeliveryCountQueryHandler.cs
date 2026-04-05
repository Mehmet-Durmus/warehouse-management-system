using MediatR;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Common.Filtering.Filters;

namespace WHMS.Application.Features.Queries.Delivery.GetDeliveryCount;

public class GetDeliveryCountQueryHandler : IRequestHandler<GetDeliveryCountQueryRequest, GetDeliveryCountQueryResponse>
{
    private readonly IDeliveryRepository _deliveryRepository;

    public GetDeliveryCountQueryHandler(IDeliveryRepository deliveryRepository)
    {
        _deliveryRepository = deliveryRepository;
    }

    public async Task<GetDeliveryCountQueryResponse> Handle(GetDeliveryCountQueryRequest request, CancellationToken cancellationToken)
    {
        DeliveryFilter filter = new()
        {
            IsReceived = request.IsReceived
        };
        
        int deliveryCount = await _deliveryRepository.GetDeliveriesCount(filter);
        
        return new() { DeliveryCount = deliveryCount };
    }
}