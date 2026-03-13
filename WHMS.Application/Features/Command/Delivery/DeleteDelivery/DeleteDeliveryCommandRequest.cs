using MediatR;

namespace WHMS.Application.Features.Command.Delivery.DeleteDelivery;

public class DeleteDeliveryCommandRequest : IRequest<DeleteDeliveryCommandResponse>
{
    public string? DeliveryId { get; set; }
}