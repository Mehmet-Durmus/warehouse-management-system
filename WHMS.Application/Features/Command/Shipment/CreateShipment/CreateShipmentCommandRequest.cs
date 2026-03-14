using MediatR;

namespace WHMS.Application.Features.Command.Shipment.CreateShipment;

public class CreateShipmentCommandRequest : IRequest<CreateShipmentCommandResponse>
{
    public string? WarehouseId { get; set; }
    public string? StoreId { get; set; }
    public DateTime ExpectedSendingDate { get; set; }
}