using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WHMS.Application.Abstractions.Infrastructure;
using WHMS.Application.Common.Constants;
using WHMS.Domain.Entities;
using WHMS.Persistence.Contexts;
using WHMS.Persistence.SeedData.Core;
using WHMS.Persistence.SeedData.Dto;

namespace WHMS.Persistence.SeedData.Seeders;

public class EmployeeSeeder : ISeeder
{
    private readonly WHMSDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJsonDataLoader _jsonDataLoader;
    private readonly SeedDataResolver _seedDataResolver;

    public EmployeeSeeder(WHMSDbContext context, IJsonDataLoader jsonDataLoader, SeedDataResolver seedDataResolver, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _jsonDataLoader = jsonDataLoader;
        _seedDataResolver = seedDataResolver;
        _userManager = userManager;
    }
    public int Order => 4;

    public async Task SeedAsync()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "SeedData", "Data", "employees.json");
        var employees = _jsonDataLoader.Load<EmployeesSeedDto>(path);
        var director = await _seedDataResolver.Employee(employees.CreatedBy);
        var random = new Random();
        DateTime date = new DateTime(2025, 02, 10).AddHours(random.Next(9, 15));

        if (!await _context.Users.AnyAsync(u => u.UserName!.Contains("WHM")))
        {
            foreach (var manager in employees.Managers)
            {
                var seq = await _context.Database
                    .SqlQuery<int>($"SELECT nextval('\"WarehouseManagerSequence\"') AS \"Value\"")
                    .SingleAsync();
                string userName = "WHM_"+seq.ToString("D4");
                ApplicationUser newManager = new();
                // {
                //     FullName = manager.FullName,
                //     UserName = userName,
                //     WarehouseId = await _seedDataResolver.WarehouseId(manager.Warehouse),
                //     IsPasswordChanged = true,
                //     PasswordChangedAt = date.AddDays(random.Next(3,5)),
                //     CreatedAt = date.AddSeconds(random.Next(0,59)),
                //     CreatedById = director!.Id,
                //     CreatedByName = director.FullName,
                //     CreatedByUserName = director.UserName,
                //     UpdatedAt = date.AddSeconds(random.Next(0,59)),
                //     UpdatedById = director!.Id,
                //     UpdatedByName = director.FullName,
                //     UpdatedByUserName = director.UserName,
                // };
                newManager.FullName = manager.FullName;
                newManager.UserName = userName;
                newManager.WarehouseId = await _seedDataResolver.WarehouseId(manager.Warehouse);
                newManager.IsPasswordChanged = true;
                newManager.PasswordChangedAt = date.AddDays(random.Next(3,5));
                newManager.CreatedAt = date.AddSeconds(random.Next(0,59));
                newManager.CreatedById = director!.Id;
                newManager.CreatedByName = director.FullName;
                newManager.CreatedByUserName = director.UserName;
                newManager.UpdatedAt = date.AddSeconds(random.Next(0,59));
                newManager.UpdatedById = director!.Id;
                newManager.UpdatedByName = director.FullName;
                newManager.UpdatedByUserName = director.UserName;
                var result = await _userManager.CreateAsync(newManager, "Manager1");
                if (result.Succeeded)
                    await _userManager.AddToRoleAsync(newManager, ApplicationRole.WarehouseManager);
                else 
                    throw new Exception("Director could not be created:" + 
                    string.Join(",", result.Errors.Select(e => e.Description)));
            }

            foreach (var staffMember in employees.Staff)
            {
                var seq = await _context.Database
                    .SqlQuery<int>($"SELECT nextval('\"WarehouseStaffSequence\"') AS \"Value\"")
                    .SingleAsync();
                string userName = "WHS_"+seq.ToString("D4");
                ApplicationUser newStaffMember = new()
                {
                    FullName = staffMember.FullName,
                    UserName = userName,
                    WarehouseId = await _seedDataResolver.WarehouseId(staffMember.Warehouse),
                    IsPasswordChanged = true,
                    PasswordChangedAt = date.AddDays(random.Next(3,5)),
                    CreatedAt = date.AddSeconds(random.Next(0,59)),
                    CreatedById = director!.Id,
                    CreatedByName = director.FullName,
                    CreatedByUserName = director.UserName,
                    UpdatedAt = date.AddSeconds(random.Next(0,59)),
                    UpdatedById = director!.Id,
                    UpdatedByName = director.FullName,
                    UpdatedByUserName = director.UserName
                };
                var result = await _userManager.CreateAsync(newStaffMember, "Staff1");
                if (result.Succeeded)
                    await _userManager.AddToRoleAsync(newStaffMember, ApplicationRole.WarehouseStaff);
                else 
                    throw new Exception("Director could not be created:" + 
                    string.Join(",", result.Errors.Select(e => e.Description)));
            }
        }

        await _context.SaveChangesAsync();
    }
}