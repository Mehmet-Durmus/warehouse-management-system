namespace WHMS.Application.Common.Filtering.Filters;

public class StoreFilter : QueryFilter
{
    public string? Name { get; set; }
    public string? CityId { get; set; }
    public string? DistrictId { get; set; }
    public string? NeighborhoodId { get; set; }
}