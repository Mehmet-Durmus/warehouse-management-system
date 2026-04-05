using MediatR;

namespace WHMS.Application.Features.Queries.Delivery.GetDeliveryCount;

public class GetDeliveryCountQueryRequest : IRequest<GetDeliveryCountQueryResponse>
{
    public bool? IsReceived { get; set; }
}