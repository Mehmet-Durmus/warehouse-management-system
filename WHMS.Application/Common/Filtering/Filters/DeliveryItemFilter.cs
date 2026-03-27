namespace WHMS.Application.Common.Filtering.Filters;

public class DeliveryItemFilter : QueryFilter
{
    public string? DeliveryId { get; set; }
    public string? SkuId { get; set; }
    public int? MaxQuantity { get; set; }
    public int? MinQuantity { get; set; }
}