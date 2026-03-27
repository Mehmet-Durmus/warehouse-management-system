using Microsoft.AspNetCore.Identity;
using WHMS.Domain.Entities;

namespace WHMS.Application.Abstractions.Persistence;

public interface IEmployeeRepository
{
    Task<List<ApplicationUser>> GetManagers();
    Task<ApplicationUser> GetManager(Guid managerId);
    Task<List<ApplicationUser>> GetStaff();
    Task<ApplicationUser> GetStaffMember(Guid staffMemberId);
    void Update(ApplicationUser employee);
    Task<bool> HasWarehouseAnyManager(Guid warehouseId);
    Task<string> GenerateWarehouseManagerUserName();
    Task<string> GenerateWarehouseStaffUserName();
    Task SetWarehouseId(Guid employeeId, Guid warehouseId);
    Task<ApplicationUser> GetEmployee(Guid employeeId);
    Task<List<ApplicationUser>> GetEmployeesByWarehouse(Guid warehouseId);
}