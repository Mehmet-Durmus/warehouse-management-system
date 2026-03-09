using MediatR;

namespace WHMS.Application.Features.Command.Delivery.CreateDeliveryItem;

public class CreateDeliveryItemCommandRequest : IRequest<CreateDeliveryItemCommandResponse>
{
    public string? DeliveryId { get; set; }
    public string? SkuId { get; set; }
    public int Quantity { get; set; }
}