using WHMS.Application.Common.Filtering.Filters;
using WHMS.Domain.Entities;

namespace WHMS.Application.Common.Filtering.Extensions;

public static class ProductCountFilterExtensions
{
    public static IQueryable<StockState> Apply(
        this IQueryable<StockState> query,
        ProductCountFilter filter
    )
    {
        return query.ApplyTotalProductCountFilter(filter);
    }

    private static IQueryable<StockState> ApplyTotalProductCountFilter(
        this IQueryable<StockState> query,
        ProductCountFilter filter
    )
    {
        return query
            .WhereIf(!string.IsNullOrWhiteSpace(filter.WarehouseId),
                s => s.WarehouseId == Guid.Parse(filter.WarehouseId!))
            
            .WhereIf(string.IsNullOrWhiteSpace(filter.WarehouseId)
                && !string.IsNullOrWhiteSpace(filter.CityId),
                s => s.Warehouse!.Address.CityId == Guid.Parse(filter.CityId!))
            
            .WhereIf(string.IsNullOrWhiteSpace(filter.WarehouseId)
                && !string.IsNullOrWhiteSpace(filter.DistrictId),
                s => s.Warehouse!.Address.DistrictId == Guid.Parse(filter.DistrictId!))
            
            .WhereIf(string.IsNullOrWhiteSpace(filter.WarehouseId)
                && !string.IsNullOrWhiteSpace(filter.NeighborhodId),
                s => s.Warehouse!.Address.NeighborhoodId == Guid.Parse(filter.NeighborhodId!));
    }
}