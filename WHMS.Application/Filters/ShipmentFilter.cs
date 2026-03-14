namespace WHMS.Application.Filters;

public class ShipmentFilter
{
    public string? WarehouseId { get; set; }
    public string? WarehouseCityId { get; set; }
    public string? WarehouseDistrictId { get; set; }
    public string? StoreId { get; set; }
    public string? StoreCityId { get; set; }
    public string? StoreDistrictId { get; set; }
    public bool? IsSent { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}