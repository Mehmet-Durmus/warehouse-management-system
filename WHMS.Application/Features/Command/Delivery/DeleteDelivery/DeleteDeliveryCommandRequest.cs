using MediatR;

namespace WHMS.Application.Features.Command.Delivery.DeleteDelivery;

public class DeleteDeliveryCommandRequest : IRequest
{
    public string? DeliveryId { get; set; }
}