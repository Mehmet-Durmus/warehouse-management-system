using WHMS.Application.Common.Filtering.Filters;
using WHMS.Domain.Entities;

namespace WHMS.Application.Common.Filtering.Extensions;

public static class ShipmentItemFilterExtensions
{
    public static IQueryable<ShipmentItem> Apply(
        this IQueryable<ShipmentItem> query,
        ShipmentItemFilter filter,
        bool applyPagination = true
    )
    {
        return query
            .ApplyShipmentItemFilter(filter)
            .ApplyCommonFilters(filter, applyPagination);
    }

    private static IQueryable<ShipmentItem> ApplyShipmentItemFilter(
        this IQueryable<ShipmentItem> query,
        ShipmentItemFilter filter
    )
    {
        return query
            .WhereIf(!string.IsNullOrWhiteSpace(filter.ShipmentId),
                i => i.ShipmentId == Guid.Parse(filter.ShipmentId!))
            .WhereIf(!string.IsNullOrWhiteSpace(filter.SkuId),
                i => i.SkuId == Guid.Parse(filter.SkuId!))
            .WhereIf(filter.MaxQuantity.HasValue,
                i => i.Quantity <= filter.MaxQuantity)
            .WhereIf(filter.MinQuantity.HasValue,
                i => i.Quantity <= filter.MinQuantity);
    }
}