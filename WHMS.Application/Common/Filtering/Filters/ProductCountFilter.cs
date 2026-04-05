namespace WHMS.Application.Common.Filtering.Filters;

public class ProductCountFilter : QueryFilter
{
    public string? WarehouseId { get; set; }
    public string? CityId { get; set; }
    public string? DistrictId { get; set; }
    public string? NeighborhodId { get; set; }
}