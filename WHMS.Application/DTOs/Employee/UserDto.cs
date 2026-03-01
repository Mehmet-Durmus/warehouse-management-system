using WHMS.Application.DTOs.Warehouse;

namespace WHMS.Application.DTOs.Employee;

public class UserDto
{
    public string? UserId { get; set; }
    public string? FullName { get; set; }
    public string? WarehouseId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}