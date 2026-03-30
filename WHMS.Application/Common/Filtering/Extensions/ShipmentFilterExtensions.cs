using System.Net.Http.Headers;
using WHMS.Application.Common.Filtering.Filters;
using WHMS.Domain.Entities;

namespace WHMS.Application.Common.Filtering.Extensions;

public static class ShipmentFilterExtensions
{
    public static IQueryable<Shipment> Apply(
        this IQueryable<Shipment> query,
        ShipmentFilter filter,
        bool applyPagination = true
    )
    {
        return query
            .ApplyShipmentFilter(filter)
            .ApplyCommonFilters(filter, applyPagination);
    }

    private static IQueryable<Shipment> ApplyShipmentFilter(
        this IQueryable<Shipment> query,
        ShipmentFilter filter
    )
    {
        return query
            .WhereIf(!string.IsNullOrWhiteSpace(filter.WarehouseId),
                s => s.WarehouseId == Guid.Parse(filter.WarehouseId!))
            
            .WhereIf(string.IsNullOrWhiteSpace(filter.WarehouseId)
                && !string.IsNullOrWhiteSpace(filter.WarehouseCityId),
                s => s.Warehouse!.Address.CityId == Guid.Parse(filter.WarehouseCityId!))
            
            .WhereIf(string.IsNullOrWhiteSpace(filter.WarehouseId)
                && !string.IsNullOrWhiteSpace(filter.WarehouseDistrictId),
                s => s.Warehouse!.Address.DistrictId == Guid.Parse(filter.WarehouseDistrictId!))
            
            .WhereIf(string.IsNullOrWhiteSpace(filter.WarehouseId)
                && !string.IsNullOrWhiteSpace(filter.WarehouseNeighborhoodId),
                s => s.Warehouse!.Address.NeighborhoodId == Guid.Parse(filter.WarehouseNeighborhoodId!))
            
            .WhereIf(!string.IsNullOrWhiteSpace(filter.StoreId),
                s => s.StoreId == Guid.Parse(filter.StoreId!))
            
            .WhereIf(string.IsNullOrWhiteSpace(filter.StoreId)
                && !string.IsNullOrWhiteSpace(filter.StoreCityId),
                s => s.Store!.Address.CityId == Guid.Parse(filter.StoreCityId!))
            
            .WhereIf(string.IsNullOrWhiteSpace(filter.StoreId)
                && !string.IsNullOrWhiteSpace(filter.StoreDistrictId),
                s => s.Store!.Address.DistrictId == Guid.Parse(filter.StoreDistrictId!))
            
            .WhereIf(string.IsNullOrWhiteSpace(filter.StoreId)
                && !string.IsNullOrWhiteSpace(filter.StoreNeighborhoodId),
                s => s.Store!.Address.NeighborhoodId == Guid.Parse(filter.StoreNeighborhoodId!))

            .WhereIf(filter.SkuIds != null && filter.SkuIds.Count > 0,
                s => s.ShipmentItems != null && s.ShipmentItems.Any(si => filter.SkuIds!.Contains(si.SkuId.ToString())))

            .WhereIf(filter.IsSent.HasValue && filter.IsSent == true,
                s => s.SendingDate != null)

            .WhereIf(filter.IsSent.HasValue && filter.IsSent == false,
                s => s.SendingDate == null)
            
            .WhereIf(filter.ExpectedSendingDateAfter.HasValue,
                s => s.ExpectedSendingDate >= filter.ExpectedSendingDateAfter)

            .WhereIf(filter.ExpectedSendingDateBefore.HasValue,
                s => s.ExpectedSendingDate <= filter.ExpectedSendingDateBefore);
            
    }
}