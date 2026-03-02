using MediatR;
using WHMS.Application.Abstractions.Persistence;

namespace WHMS.Application.Features.Queries.Employee.GetStaffMember;

public class GetStaffMemberQueryHandler : IRequestHandler<GetStaffMemberQueryRequest, GetStaffMemberQueryResponse>
{
    private readonly IEmployeeRepository _employeeRepository;

    public GetStaffMemberQueryHandler(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    public async Task<GetStaffMemberQueryResponse> Handle(GetStaffMemberQueryRequest request, CancellationToken cancellationToken)
    {
        var staffMember = await _employeeRepository.GetStaffMember(Guid.Parse(request.StaffMemberId!));
        if (staffMember is null)
            throw new Exception("Staff member not found.");

        return new()
        {
            FullName = staffMember.FullName,
            WarehouseId = staffMember.WarehouseId.ToString(),
            CreatedAt = staffMember.CreatedAt,
            UpdatedAt = staffMember.UpdatedAt
        };
    }
}