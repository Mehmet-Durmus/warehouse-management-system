namespace WHMS.Application.Features.Queries.Warehouse.GetWarehouse;

public class GetWarehouseQueryResponse
{
    public string? Id { get; set; }
    public string? City { get; set; }
    public string? District { get; set; }
    public string? Neighborhood { get; set; }
    public string? AddressLine { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}