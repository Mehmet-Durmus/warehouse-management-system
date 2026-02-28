namespace WHMS.Application.DTOs.Employee;

public class UserDto
{
    public string? FullName { get; set; }
    public Domain.Entities.Warehouse? Warehouse { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}