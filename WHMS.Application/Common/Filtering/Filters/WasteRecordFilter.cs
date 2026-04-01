namespace WHMS.Application.Common.Filtering.Filters;

public class WasteRecordFilter : QueryFilter
{
    public string? WarehouseId { get; set; }
    public string? SkuId { get; set; }
    public string? CreatedById { get; set; }
    public string? Description { get; set; }
    public int? MaxQuantity { get; set; }
    public int? MinQuantity { get; set; }
}