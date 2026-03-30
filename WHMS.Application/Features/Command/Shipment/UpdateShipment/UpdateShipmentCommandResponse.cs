namespace WHMS.Application.Features.Command.Shipment.UpdateShipment;

public class UpdateShipmentCommandResponse
{
    public string WarehouseId { get; set; } = null!;
    public string StoreId { get; set; } = null!;
    public DateTime ExpectedSendingDate { get; set; }
}