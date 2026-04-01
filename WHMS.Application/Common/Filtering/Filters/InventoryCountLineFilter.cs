namespace WHMS.Application.Common.Filtering.Filters;

public class InventoryCountLineFilter : QueryFilter
{
    public string? InventoryCountId { get; set; }
    public string? SkuId { get; set; }
    public int? MinQuantity { get; set; }
    public int? MaxQuantity { get; set; }
}