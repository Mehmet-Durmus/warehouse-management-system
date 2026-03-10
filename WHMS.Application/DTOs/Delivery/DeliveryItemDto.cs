using WHMS.Application.DTOs.Catalog;

namespace WHMS.Application.DTOs.Delivery;

public class DeliveryItemDto
{
    public string DeliveryItemId { get; set; } = null!;
    public SkuDto Sku { get; set; } = null!;
    public int Quantity { get; set; }
}