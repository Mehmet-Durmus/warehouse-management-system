using Microsoft.AspNetCore.Identity;
using WHMS.Domain.Entities;

namespace WHMS.Application.Abstractions.Persistence;

public interface IEmployeeRepository
{
    Task<List<ApplicationUser>> GetManagers();
    Task<ApplicationUser> GetManager(Guid managerId);
    void Update(ApplicationUser employee);
    Task<bool> HasWarehouseAnyManager(Guid warehouseId);
    Task<string> GenerateWarehouseManagerUserName();
    Task<string> GenerateWarehouseStaffUserName();
}