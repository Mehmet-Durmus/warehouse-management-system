namespace WHMS.Application.Features.Queries.Delivery.GetDeliveryItem;

public class GetDeliveryItemResultSkuDto
{
    public string SkuId { get; set; } = null!;
    public string SkuName { get; set; } = null!;
    public string Barcode { get; set; } = null!;
    public decimal UnitPrice { get; set; }
}