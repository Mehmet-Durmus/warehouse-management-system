using WHMS.Application.Common.Filtering.Filters;
using WHMS.Domain.Entities;

namespace WHMS.Application.Common.Filtering.Extensions;

public static class DeliveryFilterExtensions
{
    public static IQueryable<Delivery> Apply(
        this IQueryable<Delivery> query,
        DeliveryFilter filter,
        bool applyPagination = true
    )
    {
        return query
            .ApplyDeliveryFilter(filter)
            .ApplyCommonFilters(filter, applyPagination);
    }

    private static IQueryable<Delivery> ApplyDeliveryFilter(
        this IQueryable<Delivery> query,
        DeliveryFilter filter
    )
    {
        return query
            .WhereIf(!string.IsNullOrWhiteSpace(filter.WarehouseId),
                d => d.WarehouseId == Guid.Parse(filter.WarehouseId!))
            .WhereIf(string.IsNullOrWhiteSpace(filter.WarehouseId)
                && !string.IsNullOrWhiteSpace(filter.CityId),
                d => d.Warehouse!.Address.CityId == Guid.Parse(filter.CityId!))
            .WhereIf(string.IsNullOrWhiteSpace(filter.WarehouseId)
                && !string.IsNullOrWhiteSpace(filter.DistrictId),
                d => d.Warehouse!.Address.DistrictId == Guid.Parse(filter.DistrictId!))
            .WhereIf(string.IsNullOrWhiteSpace(filter.WarehouseId)
                && !string.IsNullOrWhiteSpace(filter.NeighborhoodId),
                d => d.Warehouse!.Address.NeighborhoodId == Guid.Parse(filter.NeighborhoodId!))
            .WhereIf(filter.IsReceived.HasValue && filter.IsReceived == true,
                d => d.ReceivedAt != null)
            .WhereIf(filter.IsReceived.HasValue && filter.IsReceived == false,
                d => d.ReceivedAt == null)
            .WhereIf(filter.IsReceived != false && filter.ReceivedAfter.HasValue,
                d => d.ReceivedAt >= filter.ReceivedAfter)
            .WhereIf(filter.IsReceived != false && filter.ReceivedBefore.HasValue,
                d => d.ReceivedAt <= filter.ReceivedBefore)
            .WhereIf(filter.ExpectedArrivalAfter.HasValue,
                d => d.ExpectedArrivalDate >= filter.ExpectedArrivalAfter)
            .WhereIf(filter.ExpectedArrivalBefore.HasValue,
                d => d.ExpectedArrivalDate <= filter.ExpectedArrivalBefore);
    }
}