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

    public async Task<int> SetQuantity(Guid warehouseId, Guid skuId, int delta)
        => await _context.Database.ExecuteSqlInterpolatedAsync(
            $@"UPDATE ""StockStates"" 
            SET ""Quantity"" = ""Quantity"" + {delta}
            WHERE ""WarehouseId"" = {warehouseId} 
                AND ""SkuId"" = {skuId}
                AND ""Quantity"" + {delta} >= 0");
}