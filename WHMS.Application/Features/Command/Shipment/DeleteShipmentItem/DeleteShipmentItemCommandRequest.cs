using MediatR;

namespace WHMS.Application.Features.Command.Shipment.DeleteShipmentItem;

public class DeleteShipmentItemCommandRequest : IRequest<DeleteShipmentItemCommandResponse>
{
    public string? ShipmentItemId { get; set; }
}