using System.Data.Common;

namespace WHMS.Domain.Entities;

public class DeliveryItem
{
    public Guid Id { get; set; }
    public Guid DeliveryId { get; set; }
    public Guid SkuId { get; set; }
    public SKU Sku { get; set; } = null!;
    public int Quantity { get; set; }
}