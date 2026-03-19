using WHMS.Domain.Entities;

namespace WHMS.Application.Abstractions.Persistence;

public interface IInventoryCountRepository
{
    Task CreateInventoryCount(InventoryCount inventoryCount);
    Task CreateInventoryCounLine(InventoryCountLine inventoryCountLine);
    Task<List<InventoryCount>> GetInventoryCounts();
    Task<int> CountInventoryCounts();
    Task<InventoryCount> GetInventoryCount();
    Task<List<InventoryCountLine>> GetInventoryCountLines();
    Task<int> CountInventoryCountLines();
    Task<InventoryCountLine> GetInventoryCountLine();
    void UpdateInventoryCount(InventoryCount inventoryCount);
    void UpdateInventoryCountLine(InventoryCountLine inventoryCountLine);
    Task DeleteInventoryCount(Guid inventoryCountId);
    Task DeleteInventoryCountLine(Guid inventoryCountLineId);
}