using MediatR;

namespace WHMS.Application.Features.Command.Shipment.SendShipment;

public class SendShipmentCommandRequest : IRequest<SendShipmentCommandResponse>
{
    public string? ShipmentId { get; set; }
}