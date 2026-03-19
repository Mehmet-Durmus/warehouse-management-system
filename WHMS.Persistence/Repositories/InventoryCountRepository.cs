using WHMS.Application.Abstractions.Persistence;
using WHMS.Domain.Entities;

namespace WHMS.Persistence.Repositories;

public class InventoryCountRepository : IInventoryCountRepository
{
    public Task<int> CountInventoryCountLines()
    {
        throw new NotImplementedException();
    }

    public Task<int> CountInventoryCounts()
    {
        throw new NotImplementedException();
    }

    public Task CreateInventoryCounLine(InventoryCountLine inventoryCountLine)
    {
        throw new NotImplementedException();
    }

    public Task CreateInventoryCount(InventoryCount inventoryCount)
    {
        throw new NotImplementedException();
    }

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