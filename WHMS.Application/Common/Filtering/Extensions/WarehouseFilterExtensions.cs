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
            .ApplyCommonFilters(filter, applyPagination)
            .ApplyWarehouseFilter(filter);
    }

    private static IQueryable<Warehouse> ApplyWarehouseFilter(
        this IQueryable<Warehouse> query,
        WarehouseFilter filter
    )
    {
        if (!string.IsNullOrWhiteSpace(filter.Name))
            query = query.Where(w => w.NormalizedName.Contains(filter.Name.ToUpper(CultureInfo.GetCultureInfo("tr-TR"))));
        if (!string.IsNullOrWhiteSpace(filter.CityId))
            query = query.Where(w => w.Address.CityId == Guid.Parse(filter.CityId));
        if (!string.IsNullOrWhiteSpace(filter.DistrictId))
            query = query.Where(w => w.Address.DistrictId == Guid.Parse(filter.DistrictId));
        if (!string.IsNullOrWhiteSpace(filter.NeighborhoodId))
            query = query.Where(w => w.Address.NeighborhoodId == Guid.Parse(filter.NeighborhoodId));
        
        return query;
    }
}