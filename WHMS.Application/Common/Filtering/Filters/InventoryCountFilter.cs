namespace WHMS.Application.Common.Filtering.Filters;

public class InventoryCountFilter : QueryFilter
{
    public string? WarehouseId { get; set; }
    public bool? IsDone { get; set; }
    public List<string>? SkuIds { get; set; }
}