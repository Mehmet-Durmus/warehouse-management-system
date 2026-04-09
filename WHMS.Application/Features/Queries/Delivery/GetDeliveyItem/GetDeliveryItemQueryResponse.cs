namespace WHMS.Application.Features.Queries.Delivery.GetDeliveryItem;

public class GetDeliveryItemQueryResponse
{
    public string DeliveryId { get; set; } = null!;
    public GetDeliveryItemResultSkuDto Sku { get; set; } = null!;
    public int Quantity { get; set; }
}