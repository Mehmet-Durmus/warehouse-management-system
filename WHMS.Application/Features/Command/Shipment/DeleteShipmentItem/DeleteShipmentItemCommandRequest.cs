using MediatR;

namespace WHMS.Application.Features.Command.Shipment.DeleteShipmentItem;

public class DeleteShipmentItemCommandRequest : IRequest
{
    public string? ShipmentItemId { get; set; }
}