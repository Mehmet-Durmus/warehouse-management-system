namespace WHMS.Application.Features.Queries.Employee.GetAllEmployees;

public class GetAllEmployeesResultUserDto
{
    public string EmployeeId { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string Role { get; set; } = null!;
    public string? WarehouseId { get; set; }
    public string? WarehouseName { get; set; }
}