using WHMS.Application.Abstractions.Persistence;
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

    public Task<int> CountInventoryCountLines()
    {
        throw new NotImplementedException();
    }

    public Task<int> CountInventoryCounts()
    {
        throw new NotImplementedException();
    }

    public async Task CreateInventoryCounLine(InventoryCountLine inventoryCountLine)
        => await _context.InventoryCountLines.AddAsync(inventoryCountLine);

    public async Task CreateInventoryCount(InventoryCount inventoryCount)
        => await _context.InventoryCounts.AddAsync(inventoryCount);

    public Task DeleteInventoryCount(Guid inventoryCountId)
    {
        throw new NotImplementedException();
    }

    public Task DeleteInventoryCountLine(Guid inventoryCountLineId)
    {
        throw new NotImplementedException();
    }

    public Task<InventoryCount> GetInventoryCount()
    {
        throw new NotImplementedException();
    }

    public Task<InventoryCountLine> GetInventoryCountLine()
    {
        throw new NotImplementedException();
    }

    public Task<List<InventoryCountLine>> GetInventoryCountLines()
    {
        throw new NotImplementedException();
    }

    public Task<List<InventoryCount>> GetInventoryCounts()
    {
        throw new NotImplementedException();
    }

    public void UpdateInventoryCount(InventoryCount inventoryCount)
    {
        throw new NotImplementedException();
    }

    public void UpdateInventoryCountLine(InventoryCountLine inventoryCountLine)
    {
        throw new NotImplementedException();
    }
}