namespace WHMS.Application.Filters;

public class StoreFilter
{
    public string? CityId { get; set; }
    public string? DistrictId { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}