using Microsoft.EntityFrameworkCore;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Common.Filtering.Extensions;
using WHMS.Application.Common.Filtering.Filters;
using WHMS.Domain.Entities;
using WHMS.Persistence.Contexts;

namespace WHMS.Persistence.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly WHMSDbContext _context;

    public EmployeeRepository(WHMSDbContext context)
    {
        _context = context;
    }

    public async Task<string> GenerateWarehouseManagerUserName()
    {
        var seq = await _context.Database
            .SqlQuery<int>($"SELECT nextval('\"WarehouseManagerSequence\"') AS \"Value\"")
            .SingleAsync();
        return "WHM_"+seq.ToString("D4");
    }

    public async Task<string> GenerateWarehouseStaffUserName()
    {
        var seq = await _context.Database
            .SqlQuery<int>($"SELECT nextval('\"WarehouseStaffSequence\"') AS \"Value\"")
            .SingleAsync();
        return "WHS_"+seq.ToString("D4");
    }

    public async Task<List<ApplicationUser>> GetAllEmployees(EmployeeFilter filter, bool applyPagination)
    {
        var roleId = await GetRoleId("LogisticDirector");
        return await _context.Users
            .Where(u => _context.UserRoles
                    .Any(ur => ur.UserId == u.Id && ur.RoleId != roleId))
            .Apply(filter, applyPagination)
            .Include(e => e.Warehouse)
            .ToListAsync();
    }

    public Task<ApplicationUser> GetEmployee(Guid employeeId)
    {
        throw new NotImplementedException();
    }

    public async Task<int> GetEmployeeCount(EmployeeFilter filter)
        => await _context.Users
            .Apply(filter, applyPagination: false)
            .CountAsync();

    public async Task<List<ApplicationUser>> GetEmployeesByWarehouse(Guid warehouseId)
        => await _context.Users
            .Where(u => u.WarehouseId == warehouseId)
            .ToListAsync();

    public async Task<ApplicationUser> GetManager(Guid managerId)
    {
        var roleId = await GetRoleId("WarehouseManager");

        var manager = await _context.Users
            .Where(u => u.Id == managerId)
            .Where(u => _context.UserRoles
                .Any(ur => ur.UserId == u.Id && ur.RoleId == roleId))
            .SingleOrDefaultAsync();
        
        return manager!;
    }

    public async Task<ApplicationUser> GetManagerByWarehouse(Guid warehouseId)
    {
        var roleId = await GetRoleId("WarehouseManager");
        var manager = await _context.Users
            .Where(u => u.WarehouseId == warehouseId)
            .Where(u => _context.UserRoles
                .Any(ur => ur.UserId == u.Id && ur.RoleId == roleId))
            .SingleOrDefaultAsync();
        return manager!;
    }

    public async Task<List<ApplicationUser>> GetManagers(EmployeeFilter filter, bool applyPagination)
    {
        var roleId = await GetRoleId("WarehouseManager");

        return await _context.Users
            .Apply(filter, applyPagination)
            .Where(u => _context.UserRoles
                .Any(ur => ur.UserId == u.Id && ur.RoleId == roleId))
            .ToListAsync();
            
    }

    public async Task<int> GetManagerCount(EmployeeFilter filter)
    {
        var roleId = await GetRoleId("WarehouseManager");
        return await _context.Users
            .Apply(filter, false)
            .Where(u => _context.UserRoles
                .Any(ur => ur.UserId == u.Id && ur.RoleId == roleId))
            .CountAsync();
    }

    public async Task<List<ApplicationUser>> GetStaff(EmployeeFilter filter, bool applyPagination)
    {
        var roleId = await GetRoleId("WarehouseStaff");

        return await _context.Users
            .Apply(filter, applyPagination)
            .Where(u => _context.UserRoles
                .Any(ur => ur.UserId == u.Id && ur.RoleId == roleId))
            .ToListAsync();
    }

    public async Task<int> GetStaffCount(EmployeeFilter filter)
    {
        var roleId = await GetRoleId("WarehouseStaff");

        return await _context.Users
            .Apply(filter, false)
            .Where(u => _context.UserRoles
                .Any(ur => ur.UserId == u.Id && ur.RoleId == roleId))
            .CountAsync();
    }

    public async Task<ApplicationUser> GetStaffMember(Guid staffMemberId)
    {
        var roleId = await GetRoleId("WarehouseStaff");

        var staffMember = await _context.Users
            .Where(u => u.Id == staffMemberId)
            .Where(u => _context.UserRoles
                .Any(ur => ur.UserId == u.Id && ur.RoleId == roleId))
            .SingleOrDefaultAsync();
        
        return staffMember!;
    }

    public async Task<bool> HasWarehouseAnyManager(Guid warehouseId)
    {
        return await _context.Users.AnyAsync(u => u.WarehouseId == warehouseId);
    }

    public async Task SetWarehouseId(Guid employeeId, Guid warehouseId)
    {
        var employee = await GetEmployee(employeeId);
        employee.WarehouseId = warehouseId;
    }

    public void Update(ApplicationUser employee)
    {
        _context.Update(employee);
    }

    private async Task<Guid> GetRoleId(string roleName)
    {
        return await _context.Roles
            .Where(r => r.Name == roleName)
            .Select(r => r.Id)
            .SingleAsync();
    }
}