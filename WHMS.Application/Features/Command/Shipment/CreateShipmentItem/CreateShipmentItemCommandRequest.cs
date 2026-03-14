using System.Globalization;
using MediatR;

namespace WHMS.Application.Features.Command.Shipment.CreateShipmentItem;

public class CreateShipmentItemCommandRequest : IRequest<CreateShipmentItemCommandResponse>
{
    public string? ShipmentId { get; set; }
    public string? SkuId { get; set; }
    public int Quantity { get; set; }
}