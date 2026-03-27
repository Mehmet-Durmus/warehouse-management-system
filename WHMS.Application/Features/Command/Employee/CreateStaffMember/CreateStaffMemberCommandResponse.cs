namespace WHMS.Application.Features.Command.Employee.CreateStaffMember;

public class CreateStaffMemberCommandResponse
{
    public string UserId { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string? WarehouseId { get; set; }
    public string TempPassword { get; set; } = null!;
}