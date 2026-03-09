namespace WHMS.Application.Features.Command.Delivery.CreateDeliveryItem;

public class CreateDeliveryItemCommandResponse
{
    public string Id { get; set; } = null!;
    public string DeliveryId { get; set; } = null!;
    public string SkuId { get; set; } = null!;
    public int Quantity { get; set; }
}