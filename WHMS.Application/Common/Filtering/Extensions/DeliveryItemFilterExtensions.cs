using WHMS.Application.Common.Filtering.Filters;
using WHMS.Application.Extensions;
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
        return query;
            
    }
}