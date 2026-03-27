namespace WHMS.Application.Common.Filtering.Filters;

public class EmployeeFilter : QueryFilter
{
    public string? Name { get; set; }
    public string? WarehouseId { get; set; }
    public string? CityId { get; set; }
    public string? DistrictId { get; set; }
    public string? NeighborhoodId { get; set; }
    public bool? IsAssignedToWarehouse { get; set; }
}