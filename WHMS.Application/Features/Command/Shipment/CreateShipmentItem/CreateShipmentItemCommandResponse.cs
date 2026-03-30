namespace WHMS.Application.Features.Command.Shipment.CreateShipmentItem;

public class CreateShipmentItemCommandResponse
{
    public string ShipmentItemId { get; set; } = null!;
    public string SkuId { get; set; } = null!;
    public string SkuName { get; set; } = null!;
    public int Quantity { get; set; }
}