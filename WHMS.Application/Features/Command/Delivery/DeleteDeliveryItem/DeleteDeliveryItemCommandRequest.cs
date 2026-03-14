using MediatR;

namespace WHMS.Application.Features.Command.Delivery.DeleteDeliveryItem;

public class DeleteDeliveryItemCommandRequest : IRequest<DeleteDeliveryItemCommandResponse>
{
    public string? DeliveryItemId { get; set; }
}