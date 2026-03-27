namespace WHMS.Application.Features.Queries.Warehouse.GetWarehouse;

public class GetWarehouseQueryResponse
{
    public string WarehouseName { get; set; } = null!;
    public string? ManagerId { get; set; }
    public string? ManagerUsername { get; set; }
    public string? ManagerName { get; set; }
    public int ProductCount { get; set; }
    public string CityId { get; set; } = null!;
    public string DistrictId { get; set; } = null!;
    public string NeighborhoodId { get; set; } = null!;
    public string Address { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}