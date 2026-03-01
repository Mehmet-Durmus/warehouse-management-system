using MediatR;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.DTOs.Employee;

namespace WHMS.Application.Features.Queries.Employee.GetStaff;

public class GetStaffQueryHandler : IRequestHandler<GetStaffQueryRequest, GetStaffQueryReponse>
{
    private readonly IEmployeeRepository _employeeRepository;

    public GetStaffQueryHandler(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    public async Task<GetStaffQueryReponse> Handle(GetStaffQueryRequest request, CancellationToken cancellationToken)
    {
        var staff = await _employeeRepository.GetStaff();

        var result = new GetStaffQueryReponse {Staff = []};

        foreach (var staffMember in staff)
            result.Staff.Add(new UserDto
            {
                UserId = staffMember.Id.ToString(),
                FullName = staffMember.FullName,
                WarehouseId = staffMember.WarehouseId.ToString(),
                CreatedAt = staffMember.CreatedAt,
                UpdatedAt = staffMember.UpdatedAt
            });
        
        return result;
    }
}