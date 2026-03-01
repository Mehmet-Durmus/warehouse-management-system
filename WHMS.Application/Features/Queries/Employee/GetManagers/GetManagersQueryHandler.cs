using MediatR;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.DTOs.Employee;

namespace WHMS.Application.Features.Queries.Employee.GetManagers;

public class GetManagersQueryHandler : IRequestHandler<GetManagersQueryRequest, GetManagersQueryResponse>
{
    private readonly IEmployeeRepository _employeeRepository;

    public GetManagersQueryHandler(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    public async Task<GetManagersQueryResponse> Handle(GetManagersQueryRequest request, CancellationToken cancellationToken)
    {
        var managers = await _employeeRepository.GetManagers();

        var result = new GetManagersQueryResponse { Employees = [] };

        foreach (var manager in managers)
            result.Employees.Add(new UserDto
            {
                UserId = manager.Id.ToString(),
                FullName = manager.FullName,
                WarehouseId = manager.WarehouseId.ToString(),
                CreatedAt = manager.CreatedAt,
                UpdatedAt = manager.UpdatedAt
            });

        return result;
    }
}