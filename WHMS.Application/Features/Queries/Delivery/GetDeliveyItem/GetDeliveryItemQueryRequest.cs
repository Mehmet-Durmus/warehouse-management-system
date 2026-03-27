using MediatR;

namespace WHMS.Application.Features.Queries.Delivery.GetDeliveryItem;

public class GetDeliveryItemQueryRequest : IRequest<GetDeliveryItemQueryResponse>
{
    public string? DeliveryItemId { get; set; }
}