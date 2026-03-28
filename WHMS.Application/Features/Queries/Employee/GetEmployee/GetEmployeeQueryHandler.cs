using MediatR;
using WHMS.Application.Abstractions.Infrastructure;
using WHMS.Application.Abstractions.Persistence;

namespace WHMS.Application.Features.Queries.Employee.GetEmployee;

public class GetEmployeeQueryHandler : IRequestHandler<GetEmployeeQueryRequest, GetEmployeeQueryResponse>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetEmployeeQueryHandler(IEmployeeRepository employeeRepository, ICurrentUserService currentUserService)
    {
        _employeeRepository = employeeRepository;
        _currentUserService = currentUserService;
    }

    public async Task<GetEmployeeQueryResponse> Handle(GetEmployeeQueryRequest request, CancellationToken cancellationToken)
    {
        bool isLogisticDirector = _currentUserService.Roles!.Contains("LogisticDirector");
        var employee = await _employeeRepository.GetEmployee(Guid.Parse(request.EmployeeId!));
        
        if (employee is null)
            throw new Exception("Employee not found.");

        var employeeRole = await _employeeRepository.GetEmployeeRole(employee.Id);
        
        if (!isLogisticDirector && employeeRole != "WarehouseStaff")
            throw new Exception("Employee not found.");

        if (!isLogisticDirector && employee.WarehouseId != Guid.Parse(_currentUserService.WarehouseId!))
            throw new Exception("Employee not found.");

        return new()
        {
            UserName = employee.UserName!,
            FullName = employee.FullName,
            Role = employeeRole!,
            WarehouseId = employee.WarehouseId.ToString() ?? "",
            WarehouseName = employee.Warehouse?.WarehouseName
        };
    }
}