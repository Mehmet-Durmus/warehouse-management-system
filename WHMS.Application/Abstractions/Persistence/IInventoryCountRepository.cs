using WHMS.Application.Common.Filtering.Filters;
using WHMS.Domain.Entities;

namespace WHMS.Application.Abstractions.Persistence;

public interface IInventoryCountRepository
{
    Task CreateInventoryCount(InventoryCount inventoryCount);
    Task CreateInventoryCounLine(InventoryCountLine inventoryCountLine);
    Task<List<InventoryCount>> GetInventoryCounts(InventoryCountFilter filter, bool applyPagination);
    Task<int> CountInventoryCounts(InventoryCountFilter filter);
    Task<InventoryCount> GetInventoryCount(Guid inventoryCountId);
    Task<List<InventoryCountLine>> GetInventoryCountLines(InventoryCountLineFilter filter, bool applyPagination);
    Task<int> CountInventoryCountLines(InventoryCountLineFilter filter);
    Task<InventoryCountLine> GetInventoryCountLine(Guid inventoryCountLineId);
    void UpdateInventoryCount(InventoryCount inventoryCount);
    void UpdateInventoryCountLine(InventoryCountLine inventoryCountLine);
    void DeleteInventoryCount(InventoryCount inventoryCount);
    void DeleteInventoryCountLine(InventoryCountLine inventoryCountLine);
    Task<bool> IsThereUncompletedInventoryCount(Guid warehouseId);
}