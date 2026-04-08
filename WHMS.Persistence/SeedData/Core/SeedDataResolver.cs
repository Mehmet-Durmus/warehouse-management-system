using System.Net;
using Microsoft.EntityFrameworkCore;
using WHMS.Application.Common.Constants;
using WHMS.Domain.Entities;
using WHMS.Persistence.Contexts;

namespace WHMS.Persistence.SeedData.Core;

public class SeedDataResolver
{
    private readonly WHMSDbContext _context;

    public SeedDataResolver(WHMSDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> CityId(string cityName)
        => await _context.Cities
            .Where(c => c.Name == cityName)
            .Select(c => c.CityId)
            .SingleOrDefaultAsync();
        
    public async Task<Guid> DistrictId(string cityName, string districtName)
        => await _context.Districts
            .Where(d => d.Name == districtName && d.City.Name == cityName)
            .Select(d => d.DistrictId)
            .SingleOrDefaultAsync();
        
    public async Task<Guid> NeighborhoodId(string cityName, string districtName, string neighborhoodName)
        => await _context.Neighborhoods
            .Where(n => 
                n.Name == neighborhoodName && 
                n.District.Name == districtName &&
                n.District.City.Name == cityName)
            .Select(n => n.NeighborhoodId)
            .SingleOrDefaultAsync();

    public async Task<Guid> WarehouseId(string warehouseName)
        => await _context.Warehouses
            .Where(w => w.WarehouseName == warehouseName)
            .Select(w => w.Id)
            .SingleOrDefaultAsync();
        
    public async Task<Guid> SkuId(string barcode)
        => await _context.SKUs
            .Where(s => s.Barcode == barcode)
            .Select(s => s.Id)
            .SingleOrDefaultAsync();

    public async Task<ApplicationUser?> Employee(string FullName)
        => await _context.Users
            .Where(e => e.FullName == FullName)
            .SingleOrDefaultAsync();
    
    public async Task<Guid> EmployeeId(string FullName)
        => await _context.Users
            .Where(e => e.FullName == FullName)
            .Select(e => e.Id)
            .SingleOrDefaultAsync();

    public async Task<Guid> StoreId(string storeName)
        => await _context.Stores
            .Where(s => s.StoreName == storeName)
            .Select(s => s.Id)
            .SingleOrDefaultAsync();

    public async Task<Guid> CategoryId(string categoryName)
        => await _context.Categories
            .Where(c => c.CategoryName == categoryName)
            .Select(c => c.Id)
            .SingleOrDefaultAsync();

    public async Task<ApplicationUser?> GetManagerByWarehouse(string warehouseName)
    {
        var roleId = await _context.Roles
            .Where(r => r.Name == ApplicationRole.WarehouseManager)
            .Select(r => r.Id)
            .SingleAsync();
        
        return await _context.Users
            .Where(u => u.Warehouse!.WarehouseName == warehouseName)
            .Where(u => _context.UserRoles
                .Any(ur => ur.UserId == u.Id && ur.RoleId == roleId))
            .SingleOrDefaultAsync();
    }
        
}