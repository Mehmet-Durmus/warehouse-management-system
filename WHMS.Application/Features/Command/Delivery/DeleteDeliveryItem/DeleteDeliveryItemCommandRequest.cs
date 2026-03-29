using MediatR;

namespace WHMS.Application.Features.Command.Delivery.DeleteDeliveryItem;

public class DeleteDeliveryItemCommandRequest : IRequest
{
    public string? DeliveryItemId { get; set; }
}