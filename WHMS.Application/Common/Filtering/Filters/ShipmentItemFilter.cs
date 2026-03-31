namespace WHMS.Application.Common.Filtering.Filters;

public class ShipmentItemFilter : QueryFilter
{
    public string? ShipmentId { get; set; }
    public string? SkuId { get; set; }
    public int? MaxQuantity { get; set; }
    public int? MinQuantity { get; set; }
}