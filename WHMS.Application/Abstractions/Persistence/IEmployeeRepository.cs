using Microsoft.AspNetCore.Identity;
using WHMS.Application.Common.Filtering.Extensions;
using WHMS.Application.Common.Filtering.Filters;
using WHMS.Domain.Entities;

namespace WHMS.Application.Abstractions.Persistence;

public interface IEmployeeRepository
{
    Task<List<ApplicationUser>> GetManagers(EmployeeFilter filter, bool applyPagination);
    Task<int> GetManagerCount(EmployeeFilter filter);
    Task<ApplicationUser> GetManager(Guid managerId);
    Task<List<ApplicationUser>> GetStaff(EmployeeFilter filter, bool applyPagination);
    Task<int> GetStaffCount(EmployeeFilter filter);
    Task<ApplicationUser> GetStaffMember(Guid staffMemberId);
    void Update(ApplicationUser employee);
    Task<bool> HasWarehouseAnyManager(Guid warehouseId);
    Task<string> GenerateWarehouseManagerUserName();
    Task<string> GenerateWarehouseStaffUserName();
    Task SetWarehouseId(Guid employeeId, Guid warehouseId);
    Task<ApplicationUser> GetEmployee(Guid employeeId);
    Task<List<ApplicationUser>> GetEmployeesByWarehouse(Guid warehouseId);
    Task<ApplicationUser> GetManagerByWarehouse(Guid warehouseId);
    Task<List<ApplicationUser>> GetAllEmployees(EmployeeFilter filter, bool applyPagination);
    Task<int> GetEmployeeCount(EmployeeFilter filter);
    Task<Dictionary<Guid, string>> GetEmployeeRoles(IEnumerable<Guid> employeeIds);
    Task<string?> GetEmployeeRole(Guid employeeId);
}