using MediatR;

namespace WHMS.Application.Features.Command.Shipment.DeleteShipment;

public class DeleteShipmentCommandRequest : IRequest
{
    public string? ShipmentId { get; set; }
}