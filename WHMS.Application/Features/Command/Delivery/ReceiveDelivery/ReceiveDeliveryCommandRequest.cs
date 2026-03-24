using MediatR;

namespace WHMS.Application.Features.Command.Delivery.ReceiveDelivery;

public class ReceiveDeliveryCommandRequest : IRequest<ReceiveDeliveryCommandResponse>
{
    public string? DeliveryId { get; set; }
}