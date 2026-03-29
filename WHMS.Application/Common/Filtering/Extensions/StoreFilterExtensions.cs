using System.Globalization;
using WHMS.Application.Common.Filtering.Filters;
using WHMS.Domain.Entities;

namespace WHMS.Application.Common.Filtering.Extensions;

public static class StoreFilterExtensions
{
    public static IQueryable<Store> Apply(
        this IQueryable<Store> query,
        StoreFilter filter,
        bool applyPagination = true
    )
    {
        return query
            .ApplyStoreFilter(filter)
            .ApplyCommonFilters(filter, applyPagination);
    }


    private static IQueryable<Store> ApplyStoreFilter(
        this IQueryable<Store> query,
        StoreFilter filter
    )
    {
        return query
            .WhereIf(!string.IsNullOrWhiteSpace(filter.Name),
                s => s.NormalizedName.Contains(filter.Name!.ToUpper(CultureInfo.GetCultureInfo("tr-TR"))))
            .WhereIf(!string.IsNullOrWhiteSpace(filter.CityId),
                s => s.Address.CityId == Guid.Parse(filter.CityId!))
            .WhereIf(!string.IsNullOrWhiteSpace(filter.DistrictId),
                s => s.Address.DistrictId == Guid.Parse(filter.DistrictId!))
            .WhereIf(!string.IsNullOrWhiteSpace(filter.NeighborhoodId),
                s => s.Address.NeighborhoodId == Guid.Parse(filter.NeighborhoodId!));
    }
}