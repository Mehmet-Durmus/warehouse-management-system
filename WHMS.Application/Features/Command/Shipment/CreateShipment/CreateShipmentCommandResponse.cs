namespace WHMS.Application.Features.Command.Shipment.CreateShipment;

public class CreateShipmentCommandResponse
{
    public string ShipmentId { get; set; } = null!;
    public string WarehouseId { get; set; } = null!;
    public string StoreId { get; set; } = null!;
    public DateTime ExpectedSendingDate { get; set; }
}