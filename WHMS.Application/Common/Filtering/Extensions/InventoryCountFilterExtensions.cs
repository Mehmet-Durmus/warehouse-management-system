using WHMS.Application.Common.Filtering.Filters;
using WHMS.Domain.Entities;

namespace WHMS.Application.Common.Filtering.Extensions;

public static class InventoryCountFilterExtensions
{
    public static IQueryable<InventoryCount> Apply(
        this IQueryable<InventoryCount> query,
        InventoryCountFilter filter,
        bool applyPagination = true
    )
    {
        return query
            .ApplyInventoryCountFilter(filter)
            .ApplyCommonFilters(filter, applyPagination);
    }

    private static IQueryable<InventoryCount> ApplyInventoryCountFilter(
        this IQueryable<InventoryCount> query,
        InventoryCountFilter filter
    )
    {
        return query
            .WhereIf(!string.IsNullOrWhiteSpace(filter.WarehouseId),
                c => c.WarehouseId == Guid.Parse(filter.WarehouseId!))
            
            .WhereIf(filter.IsDone.HasValue && filter.IsDone == true,
                c => c.IsCompleted)

            .WhereIf(filter.IsDone.HasValue && filter.IsDone == false,
                c => !c.IsCompleted)

            .WhereIf(filter.SkuIds != null && filter.SkuIds.Count > 0,
                c => c.InventoryCountLines != null && c.InventoryCountLines.Any(cl => filter.SkuIds!.Contains(cl.SkuId.ToString())));
    }
}