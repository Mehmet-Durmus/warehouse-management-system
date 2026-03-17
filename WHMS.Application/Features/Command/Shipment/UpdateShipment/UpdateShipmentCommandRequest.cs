using MediatR;

namespace WHMS.Application.Features.Command.Shipment.UpdateShipment;

public class UpdateShipmentCommandRequest : IRequest<UpdateShipmentCommandResponse>
{
    public string? ShipmentId { get; set; }
    public string? WarehouseId { get; set; }
    public string? StoreId { get; set; }
    public DateTime ExpectedSendingDate { get; set; }
}