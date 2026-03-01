using MediatR;

namespace WHMS.Application.Features.Command.Employee.CreateStaffMember;

public class CreateStaffMemberCommandRequest : IRequest<CreateStaffMemberCommandResponse>
{
    public string? FullName { get; set; }
    public string? WarehouseId { get; set; }
}