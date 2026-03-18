using MediatR;

namespace WHMS.Application.Features.Command.Shipment.UpdateShipmentItem;

public class UpdateShipmentItemCommandRequest : IRequest<UpdateShipmentItemCommandResponse>
{
    public string? ShipmentItemId { get; set; }
    public string? ShipmentId { get; set; }
    public string? SkuId { get; set; }
    public int Quantity { get; set; }
}