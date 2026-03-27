using WHMS.Domain.Entities.Abstractions;

namespace WHMS.Domain.Entities;

public class StockState
{
    public Guid WarehouseId { get; set; }
    public Warehouse? Warehouse { get; set; }
    public Guid SkuId { get; set; }
    public SKU? Sku { get; set; }
    public int Quantity { get; set; }
}