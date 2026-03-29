namespace WHMS.Application.Features.Command.Delivery.UpdateDeliveryItem;

public class UpdateDeliveryItemCommandResponse
{
    public string DeliveryId { get; set; } = null!;
    public string SkuId { get; set; } = null!;
    public string SkuName { get; set; } = null!;
    public int Quantity { get; set; }
}