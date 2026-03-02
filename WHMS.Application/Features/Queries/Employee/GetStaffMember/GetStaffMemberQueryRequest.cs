using MediatR;

namespace WHMS.Application.Features.Queries.Employee.GetStaffMember;

public class GetStaffMemberQueryRequest : IRequest<GetStaffMemberQueryResponse>
{
    public string? StaffMemberId { get; set; }
}