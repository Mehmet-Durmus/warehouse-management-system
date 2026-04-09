namespace WHMS.Application.Features.Queries.Employee.GetEmployee;

public class GetEmployeeQueryResponse
{
    public string EmployeeId { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string Role { get; set; } = null!;
    public string? WarehouseId { get; set; }
    public string? WarehouseName { get; set; }
}