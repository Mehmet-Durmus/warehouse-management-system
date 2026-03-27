using System.Data.Common;
using System.Globalization;
using WHMS.Application.Common.Filtering.Filters;
using WHMS.Domain.Entities;

namespace WHMS.Application.Common.Filtering.Extensions;

public static class WarehouseFilterExtensions
{
    public static IQueryable<Warehouse> Apply(
        this IQueryable<Warehouse> query,
        WarehouseFilter filter,
        bool applyPagination = true
    )
    {
        return query
            .ApplyWarehouseFilter(filter)
            .ApplyCommonFilters(filter, applyPagination);
    }

    private static IQueryable<Warehouse> ApplyWarehouseFilter(
        this IQueryable<Warehouse> query,
        WarehouseFilter filter
    )
    {   
        return query
            .WhereIf(!string.IsNullOrWhiteSpace(filter.Name),
                w => w.NormalizedName.Contains(filter.Name!.ToUpper(CultureInfo.GetCultureInfo("tr-TR"))))
            .WhereIf(!string.IsNullOrWhiteSpace(filter.CityId),
                w => w.Address.CityId == Guid.Parse(filter.CityId!))
            .WhereIf(!string.IsNullOrWhiteSpace(filter.DistrictId),
                w => w.Address.DistrictId == Guid.Parse(filter.DistrictId!))
            .WhereIf(!string.IsNullOrWhiteSpace(filter.NeighborhoodId),
                w => w.Address.NeighborhoodId == Guid.Parse(filter.NeighborhoodId!))
            .WhereIf(filter.SkuIds is not null && filter.SkuIds.Count > 0,
                w => w.StockStates.Any(ss => filter.SkuIds!.Contains(ss.SkuId.ToString())));
    }
}