using Microsoft.EntityFrameworkCore;
using WHMS.Application.Abstractions.Persistence;
using WHMS.Persistence.Contexts;
using WHMS.Persistence.Migrations;

namespace WHMS.Persistence.Repositories;

public class StockStateRepository : IStockStateRepository
{
    private readonly WHMSDbContext _context;

    public StockStateRepository(WHMSDbContext context)
    {
        _context = context;
    }

    public async Task<bool> HasStockInAnyWarehouse(Guid skuId)
        => await _context.StockStates
            .AnyAsync(ss => ss.SkuId == skuId && ss.Quantity > 0);

    public async Task<int> SetQuantity(Guid warehouseId, Guid skuId, int quantity)
        => await _context.Database.ExecuteSqlInterpolatedAsync(
            $@"INSERT INTO ""StockStates"" (""WarehouseId"", ""SkuId"", ""Quantity"")
            VALUES ({warehouseId}, {skuId}, {quantity})
            ON CONFLICT (""WarehouseId"", ""SkuId"")
            DO UPDATE SET ""Quantity"" = {quantity}");

    public async Task<int> UpdateQuantity(Guid warehouseId, Guid skuId, int delta)
        => await _context.Database.ExecuteSqlInterpolatedAsync(
            $@"INSERT INTO ""StockStates"" (""WarehouseId"", ""SkuId"", ""Quantity"")
            VALUES ({warehouseId}, {skuId}, {delta})
            ON CONFLICT (""WarehouseId"", ""SkuId"")
            DO UPDATE SET ""Quantity"" = ""StockStates"".""Quantity"" + {delta}
            WHERE ""StockStates"".""Quantity"" + {delta} >= 0");

}