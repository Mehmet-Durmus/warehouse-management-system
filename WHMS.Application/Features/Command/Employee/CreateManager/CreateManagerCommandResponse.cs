namespace WHMS.Application.Features.Command.Employee.CreateEmployee;

public class CreateManagerCommandResponse
{
    public string UserId { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string? WarehouseId { get; set; }
    public string TempPassword { get; set; } = null!;
}