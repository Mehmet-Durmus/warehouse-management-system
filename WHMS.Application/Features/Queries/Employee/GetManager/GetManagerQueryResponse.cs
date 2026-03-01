namespace WHMS.Application.Features.Queries.Employee.GetManager;

public class GetManagerQueryResponse
{
    public string? FullName { get; set; }
    public string? WarehouseId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}