using WHMS.Application.Common.Filtering.Filters;
using WHMS.Domain.Entities;

namespace WHMS.Application.Abstractions.Persistence;

public interface IStockStateRepository
{
    Task<int> UpdateQuantity(Guid warehouseId, Guid skuId, int delta);
    Task<int> SetQuantity(Guid warehouseId, Guid skuId, int quantity);
    Task<bool> HasStockInAnyWarehouse(Guid skuId);
    Task<int> TotalStockByWarehouse(Guid warehouseId);
    Task<int> GetStockQuantity(Guid warehouseId, Guid skuId);
    Task<int> GetTotalProductCount(ProductCountFilter filter);
    Task<List<StockState>> GetProductCounts(ProductCountFilter filter);
}