using WHMS.Application.Common.Filtering.Filters;
using WHMS.Domain.Entities;

namespace WHMS.Application.Common.Filtering.Extensions;


public static class DeliveryItemFilterExtensions
{
    public static IQueryable<DeliveryItem> Apply(
        this IQueryable<DeliveryItem> query,
        DeliveryItemFilter filter,
        bool applyPagination = true
    )
    {
        return query
            .ApplyDeliveryItemFilter(filter)
            .ApplyCommonFilters(filter, applyPagination);
    }

    private static IQueryable<DeliveryItem> ApplyDeliveryItemFilter(
        this IQueryable<DeliveryItem> query,
        DeliveryItemFilter filter
    )
    {
        return query
            .WhereIf(!string.IsNullOrWhiteSpace(filter.DeliveryId),
                i => i.DeliveryId == Guid.Parse(filter.DeliveryId!))
            .WhereIf(!string.IsNullOrWhiteSpace(filter.SkuId),
                i => i.SkuId == Guid.Parse(filter.SkuId!))
            .WhereIf(filter.MaxQuantity.HasValue,
                i => i.Quantity <= filter.MaxQuantity)
            .WhereIf(filter.MinQuantity.HasValue,
                i => i.Quantity <= filter.MinQuantity);
    }
}