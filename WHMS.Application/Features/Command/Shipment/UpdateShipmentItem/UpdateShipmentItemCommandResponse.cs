namespace WHMS.Application.Features.Command.Shipment.UpdateShipmentItem;

public class UpdateShipmentItemCommandResponse
{
    public string SkuId { get; set; } = null!;
    public string SkuName { get; set; } = null!;
    public int Quantity { get; set; }
}