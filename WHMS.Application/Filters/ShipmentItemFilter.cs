namespace WHMS.Application.Filters;

public class ShipmentItemFilter
{
    public string? ShipmentId { get; set; }
    public string? SkuId { get; set; }
    public int? MaxQuantity { get; set; }
    public int? MinQuantity { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}