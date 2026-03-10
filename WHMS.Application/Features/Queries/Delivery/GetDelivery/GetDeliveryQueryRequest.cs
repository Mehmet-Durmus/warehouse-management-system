using MediatR;

namespace WHMS.Application.Features.Queries.Delivery.GetDelivery;

public class GetDeliveryQueryRequet : IRequest<GetDeliveryQueryResponse>
{
    public string? DeliveryId { get; set; }
}