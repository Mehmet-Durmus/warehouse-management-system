using WHMS.Application.DTOs.Catalog;

namespace WHMS.Application.DTOs.Shipment;

public class ShipmentItemDto
{
    public string ShipmentItemId { get; set; } = null!;
    public SkuDto Sku { get; set; } = null!;
    public int Quantity { get; set; }
}