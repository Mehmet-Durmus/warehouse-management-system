using MediatR;

namespace WHMS.Application.Features.Command.Employee.UpdateStaffMember;

public class UpdateStaffMemberCommandRequest : IRequest<UpdateStaffMemberCommandResponse>
{
    public string? UserId { get; set; }
    public string? FullName { get; set; }
    public string? WarehouseId { get; set; }
}