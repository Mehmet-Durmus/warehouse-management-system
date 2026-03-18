using MediatR;

namespace WHMS.Application.Features.Command.Shipment.DeleteShipment;

public class DeleteShipmentCommandRequest : IRequest<DeleteShipmentCommandResponse>
{
    public string? ShipmentId { get; set; }
}