using Microsoft.EntityFrameworkCore;
using WHMS.Application.Abstractions.Persistence;
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

    public async Task<ApplicationUser> GetManager(Guid managerId)
    {
        var roleId = await _context.Roles
            .Where(r => r.Name == "WarehouseManager")
            .Select(r => r.Id)
            .SingleAsync();

        var manager = await _context.Users
            .Where(u => u.Id == managerId)
            .Where(u => _context.UserRoles
                .Any(ur => ur.UserId == u.Id && ur.RoleId == roleId))
            .SingleOrDefaultAsync();
        
        return manager!;
    }

    public async Task<List<ApplicationUser>> GetManagers()
    {
        var roleId = await _context.Roles
            .Where(r => r.Name == "WarehouseManager")
            .Select(r => r.Id)
            .SingleAsync();

        return await _context.Users
            .Where(u => _context.UserRoles
                .Any(ur => ur.UserId == u.Id && ur.RoleId == roleId))
            .ToListAsync();
            
    }

    public async Task<bool> HasWarehouseAnyManager(Guid warehouseId)
    {
        return await _context.Users.AnyAsync(u => u.WarehouseId == warehouseId);
    }

    public void Update(ApplicationUser employee)
    {
        _context.Update(employee);
    }
}