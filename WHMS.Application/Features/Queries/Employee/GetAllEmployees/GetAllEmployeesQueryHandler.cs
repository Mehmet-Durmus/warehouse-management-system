using MediatR;
using Microsoft.AspNetCore.Identity;
using WHMS.Application.Abstractions.Infrastructure;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Common.Constants;
using WHMS.Application.Common.Filtering.Extensions;
using WHMS.Application.Common.Filtering.Filters;
using WHMS.Domain.Entities;

namespace WHMS.Application.Features.Queries.Employee.GetAllEmployees;

public class GetAllEmployeesQueryHandler : IRequestHandler<GetAllEmployeesQueryRequest, GetAllEmployeesQueryResponse>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICurrentUserService _currentUserService;

    public GetAllEmployeesQueryHandler(IEmployeeRepository employeeRepository, UserManager<ApplicationUser> userManager, ICurrentUserService currentUserService)
    {
        _employeeRepository = employeeRepository;
        _userManager = userManager;
        _currentUserService = currentUserService;
    }

    public async Task<GetAllEmployeesQueryResponse> Handle(GetAllEmployeesQueryRequest request, CancellationToken cancellationToken)
    {
        bool isLogisticDirector = _currentUserService.Roles!.Contains(ApplicationRole.LogisticDirector);

        // CurrnetUser Manager ise sadece staff alabilir, Director ise filtre uygulayabilir
        request.IsManager = isLogisticDirector ? request.IsManager : false;

        EmployeeFilter filter = new()
        {
            Name = request.Name,
            CreatedAfter = request.CreatedAfter,
            CreatedBefore = request.CreatedBefore,
            UpdatedAfter = request.UpdatedAfter,
            UpdatedBefore = request.UpdatedBefore,
            Page = request.Page,
            PageSize = request.PageSize
        };

        if (isLogisticDirector)
        {
            filter.WarehouseId = request.WarehouseId;
            filter.CityId = request.CityId;
            filter.DistrictId = request.DistrictId;
            filter.NeighborhoodId = request.NeighborhoodId;
            filter.IsAssignedToWarehouse = request.IsAssignedToWarehouse;  
        }
        else
            filter.WarehouseId = _currentUserService.WarehouseId;

        List<ApplicationUser> employees;
        int employeeCount;

        if (request.IsManager == true)
        {
            employees = await _employeeRepository.GetManagers(filter, applyPagination: true);
            employeeCount = await _employeeRepository.GetEmployeeCount(filter);
        }
        else if (request.IsManager == false)
        {
            employees = await _employeeRepository.GetStaff(filter, applyPagination: true);
            employeeCount = await _employeeRepository.GetStaffCount(filter);
        }
        else
        {
            employees = await _employeeRepository.GetAllEmployees(filter, applyPagination: true);
            employeeCount = await _employeeRepository.GetEmployeeCount(filter);          
        }


        GetAllEmployeesQueryResponse response = new() { Employees = [] };
        response.Pagination = new(
            request.Page,
            request.PageSize,
            (int)Math.Ceiling((double) employeeCount / request.PageSize)
        );

        var roles = await _employeeRepository.GetEmployeeRoles(employees.Select(e => e.Id));

        foreach (var employee in employees)
            response.Employees.Add(new()
            {
                UserId = employee.Id.ToString(),
                UserName = employee.UserName!,
                FullName = employee.FullName,
                Role = roles.GetValueOrDefault(employee.Id)!,
                WarehouseId = employee.WarehouseId.ToString(),
                WarehouseName = employee.Warehouse?.WarehouseName
            });

        return response;
    }
}