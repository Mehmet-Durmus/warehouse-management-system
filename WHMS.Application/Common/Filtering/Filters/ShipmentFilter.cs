namespace WHMS.Application.Common.Filtering.Filters;

public class ShipmentFilter : QueryFilter
{
    public string? WarehouseId { get; set; }
    public string? WarehouseCityId { get; set; }
    public string? WarehouseDistrictId { get; set; }
    public string? WarehouseNeighborhoodId { get; set; }
    public string? StoreId { get; set; }
    public string? StoreCityId { get; set; }
    public string? StoreDistrictId { get; set; }
    public string? StoreNeighborhoodId { get; set; }
    public List<string>? SkuIds { get; set; }
    public bool? IsSent { get; set; }
    public DateTime? SentAfter { get; set; }
    public DateTime? SentBefore { get; set; }
    public DateTime? ExpectedSendingDateAfter { get; set; }
    public DateTime? ExpectedSendingDateBefore { get; set; }
}