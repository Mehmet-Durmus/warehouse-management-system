using WHMS.Application.Common.Filtering.Filters;
using WHMS.Domain.Entities;

namespace WHMS.Application.Common.Filtering.Extensions;

public static class InventoryCountLineFilterExtensions
{
    public static IQueryable<InventoryCountLine> Apply(
        this IQueryable<InventoryCountLine> query,
        InventoryCountLineFilter filter,
        bool applyPagination
    )
    {
        return query
            .ApplyInventoryCountLineFilter(filter)
            .ApplyCommonFilters(filter, applyPagination);
    }

    private static IQueryable<InventoryCountLine> ApplyInventoryCountLineFilter(
        this IQueryable<InventoryCountLine> query,
        InventoryCountLineFilter filter
    )
    {
        return query
            .WhereIf(!string.IsNullOrWhiteSpace(filter.InventoryCountId),
                l => l.InventoryCountId == Guid.Parse(filter.InventoryCountId!))

            .WhereIf(!string.IsNullOrWhiteSpace(filter.SkuId),
                l => l.SkuId == Guid.Parse(filter.SkuId!))

            .WhereIf(filter.MaxQuantity.HasValue,
                l => l.Quantity <= filter.MaxQuantity)
            
            .WhereIf(filter.MinQuantity.HasValue,
                l => l.Quantity >= filter.MinQuantity);
    }
}