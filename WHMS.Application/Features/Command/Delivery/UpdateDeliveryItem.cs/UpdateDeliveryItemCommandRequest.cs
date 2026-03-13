using MediatR;

namespace WHMS.Application.Features.Command.Delivery.UpdateDeliveryItem;

public class UpdateDeliveryItemCommandRequest : IRequest<UpdateDeliveryItemCommandResponse>
{
    public string? DeliveryItemId { get; set; }
    public string? DeliveryId { get; set; }
    public string? SkuId { get; set; }
    public int Quantity { get; set; }
}