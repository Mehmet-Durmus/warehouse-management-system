using Microsoft.EntityFrameworkCore;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Application.Extensions;
using WHMS.Application.Filters;
using WHMS.Domain.Entities;
using WHMS.Persistence.Contexts;

namespace WHMS.Persistence.Repositories;

public class InventoryCountRepository : IInventoryCountRepository
{
    private readonly WHMSDbContext _context;

    public InventoryCountRepository(WHMSDbContext context)
    {
        _context = context;
    }

    public async Task<int> CountInventoryCountLines(InventoryCountLineFilter filter)
        => await _context.InventoryCountLines   
            .ApplyInventoryCountLineFilter(filter, withPagination: false)
            .CountAsync();
    

    public async Task<int> CountInventoryCounts(InventoryCountFilter filter)
        => await _context.InventoryCounts
            .ApplyInventoryCountFilter(filter, false)
            .CountAsync();

    public async Task CreateInventoryCounLine(InventoryCountLine inventoryCountLine)
        => await _context.InventoryCountLines.AddAsync(inventoryCountLine);

    public async Task CreateInventoryCount(InventoryCount inventoryCount)
        => await _context.InventoryCounts.AddAsync(inventoryCount);

    public void DeleteInventoryCount(InventoryCount inventoryCount)
        => inventoryCount.IsActive = false;

    public void DeleteInventoryCountLine(InventoryCountLine inventoryCountLine)
        => inventoryCountLine.IsActive = false;

    public async Task<InventoryCount> GetInventoryCount(Guid inventoryCountId)
    {
        var inventoryCount = await _context.InventoryCounts
            .Where(i => i.Id == inventoryCountId)
            .Include(i => i.InventoryCountLines)!
            .ThenInclude(l => l.Sku)
            .SingleOrDefaultAsync();

        return inventoryCount!;
    }

    public async Task<InventoryCountLine> GetInventoryCountLine(Guid inventoryCountLineId)
    {
        var inventoryCountLine = await _context.InventoryCountLines.FindAsync(inventoryCountLineId);
        return inventoryCountLine!;
    }

    public async Task<List<InventoryCountLine>> GetInventoryCountLines(InventoryCountLineFilter filter, bool withPagination)
        => await _context.InventoryCountLines
            .ApplyInventoryCountLineFilter(filter, withPagination)
            .Include(l => l.Sku)
            .ToListAsync();

    public async Task<List<InventoryCount>> GetInventoryCounts(InventoryCountFilter filter, bool withPagination)
        => await _context.InventoryCounts
            .ApplyInventoryCountFilter(filter, withPagination)
            .Include(i => i.InventoryCountLines)!
                .ThenInclude(l => l.Sku)
            .ToListAsync();

    public async Task<bool> IsThereUncomplatedInventoryCount(Guid warehouseId)
        => await _context.InventoryCounts
            .Where(ic => ic.WarehouseId == warehouseId && !ic.IsComplated)
            .AnyAsync();

    public void UpdateInventoryCount(InventoryCount inventoryCount)
        => _context.InventoryCounts.Update(inventoryCount);

    public void UpdateInventoryCountLine(InventoryCountLine inventoryCountLine)
    {
        throw new NotImplementedException();
    }
}