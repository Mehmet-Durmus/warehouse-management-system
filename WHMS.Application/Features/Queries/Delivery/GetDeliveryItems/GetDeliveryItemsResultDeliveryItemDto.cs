namespace WHMS.Application.Features.Queries.Delivery.GetDeliveryItems;

public class GetDeliveryItemsResultDeliveryItemDto
{
    public string DeliveryItemId { get; set; } = null!;
    public GetDeliveryItemsResultSkuDto Sku { get; set; } = null!;
    public int Quantity { get; set; }
}