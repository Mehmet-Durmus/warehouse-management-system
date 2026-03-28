using MediatR;
using Microsoft.AspNetCore.Identity;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Common.Filtering.Extensions;
using WHMS.Application.Common.Filtering.Filters;
using WHMS.Domain.Entities;

namespace WHMS.Application.Features.Queries.Employee.GetAllEmployees;

public class GetAllEmployeesQueryHandler : IRequestHandler<GetAllEmployeesQueryRequest, GetAllEmployeesQueryResponse>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly UserManager<ApplicationUser> _userManager;

    public GetAllEmployeesQueryHandler(IEmployeeRepository employeeRepository, UserManager<ApplicationUser> userManager)
    {
        _employeeRepository = employeeRepository;
        _userManager = userManager;
    }

    public async Task<GetAllEmployeesQueryResponse> Handle(GetAllEmployeesQueryRequest request, CancellationToken cancellationToken)
    {
        EmployeeFilter filter = new()
        {
            Name = request.Name,
            WarehouseId = request.WarehouseId,
            CityId = request.CityId,
            DistrictId = request.DistrictId,
            NeighborhoodId = request.NeighborhoodId,
            IsAssignedToWarehouse = request.IsAssignedToWarehouse,
            CreatedAfter = request.CreatedAfter,
            CreatedBefore = request.CreatedBefore,
            UpdatedAfter = request.UpdatedAfter,
            UpdatedBefore = request.UpdatedBefore,
            Page = request.Page,
            PageSize = request.PageSize
        };

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

        foreach (var employee in employees)
            response.Employees.Add(new()
            {
                UserId = employee.Id.ToString(),
                UserName = employee.UserName!,
                FullName = employee.FullName,
                Role = (await _userManager.GetRolesAsync(employee)).FirstOrDefault()!,
                WarehouseId = employee.WarehouseId.ToString(),
                WarehouseName = employee.Warehouse?.WarehouseName
            });

        return response;
    }
}