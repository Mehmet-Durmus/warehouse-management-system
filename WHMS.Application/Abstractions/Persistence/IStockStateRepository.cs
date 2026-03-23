namespace WHMS.Application.Abstractions.Persistence;

public interface IStockStateRepository
{
    Task<int> SetQuantity(Guid warehouseId, Guid skuId, int delta);
}