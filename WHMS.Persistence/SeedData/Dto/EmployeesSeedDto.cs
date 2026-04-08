namespace WHMS.Persistence.SeedData.Dto;

public class EmployeesSeedDto
{
    public string CreatedBy { get; set; } = null!;
    public List<Employee> Managers { get; set; } = null!;
    public List<Employee> Staff { get; set; } = null!;
}

public class Employee
{
    public string FullName { get; set; } = null!;
    public string Warehouse { get; set; } = null!;
} 