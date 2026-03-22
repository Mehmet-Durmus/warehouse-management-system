using System.Linq.Expressions;
using WHMS.Application.Filters;
using WHMS.Domain.Entities;

namespace WHMS.Application.Extensions;

public static class QueryableExtensions
{

    public static IQueryable<WasteRecord> ApplyWasteRecordFilter(
        this IQueryable<WasteRecord> query,
        WasteRecordFilter filter,
        bool withPagination = true
    )
    {
        query = query
            .WhereIf(!string.IsNullOrWhiteSpace(filter.WarehouseId), w => w.WarehouseId == Guid.Parse(filter.WarehouseId!))
            .WhereIf(!string.IsNullOrWhiteSpace(filter.SkuId), w => w.SkuId == Guid.Parse(filter.SkuId!))
            .WhereIf(filter.MinQuantity.HasValue, w => w.Quantity >= filter.MinQuantity)
            .WhereIf(filter.MaxQuantity.HasValue, w => w.Quantity <= filter.MaxQuantity);

        if (withPagination)
            query = query
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize);
        
        return query; 
    }

    public static IQueryable<InventoryCountLine> ApplyInventoryCountLineFilter(
        this IQueryable<InventoryCountLine> query,
        InventoryCountLineFilter filter,
        bool withPagination = true
    )
    {
        query = query
            .WhereIf(!string.IsNullOrWhiteSpace(filter.InventoryCountId), l => l.InventoryCountId == Guid.Parse(filter.InventoryCountId!))
            .WhereIf(!string.IsNullOrWhiteSpace(filter.SkuId), l => l.SkuId == Guid.Parse(filter.SkuId!))
            .WhereIf(filter.MinQuantity.HasValue, l => l.Quantity >= filter.MinQuantity)
            .WhereIf(filter.MaxQuantity.HasValue, l => l.Quantity <= filter.MaxQuantity);

        if (withPagination)
            query = query
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize);
        
        return query; 
    }

    public static IQueryable<InventoryCount> ApplyInventoryCountFilter(
        this IQueryable<InventoryCount> query,
        InventoryCountFilter filter,
        bool withPagination = true
    )
    {
        query = query
            .WhereIf(!string.IsNullOrWhiteSpace(filter.WarehouseId), i => i.WarehouseId == Guid.Parse(filter.WarehouseId!));
        
        if (withPagination)
            query = query
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize);
        
        return query;   
    }

    public static IQueryable<ShipmentItem> ApplyShipmentItemFilter(
        this IQueryable<ShipmentItem> query,
        ShipmentItemFilter filter,
        bool withPagination = true
    )
    {
        query = query
            .WhereIf(!string.IsNullOrWhiteSpace(filter.ShipmentId), i => i.ShipmentId == Guid.Parse(filter.ShipmentId!))
            .WhereIf(!string.IsNullOrWhiteSpace(filter.SkuId), i => i.SkuId == Guid.Parse(filter.SkuId!))
            .WhereIf(filter.MinQuantity.HasValue, i => i.Quantity >= filter.MinQuantity)
            .WhereIf(filter.MaxQuantity.HasValue, i => i.Quantity <= filter.MaxQuantity);

        if (withPagination)
            query = query
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize);
        
        return query;   
    }

    public static IQueryable<Shipment> ApplyShipmentFilter(
        this IQueryable<Shipment> query,
        ShipmentFilter filter,
        bool withPagination = true
    )
    {
        query = query
            .WhereIf(!string.IsNullOrWhiteSpace(filter.WarehouseId), s => s.WarehouseId == Guid.Parse(filter.WarehouseId!))
            .WhereIf(string.IsNullOrWhiteSpace(filter.WarehouseId) &&
                !string.IsNullOrWhiteSpace(filter.WarehouseCityId),
                s => s.Warehouse!.Address.CityId == Guid.Parse(filter.WarehouseCityId!))
            .WhereIf(string.IsNullOrWhiteSpace(filter.WarehouseId) &&
                !string.IsNullOrWhiteSpace(filter.WarehouseDistrictId),
                s => s.Warehouse!.Address.DistrictId == Guid.Parse(filter.WarehouseDistrictId!))
            .WhereIf(!string.IsNullOrWhiteSpace(filter.StoreId), s => s.StoreId == Guid.Parse(filter.StoreId!))
            .WhereIf(string.IsNullOrWhiteSpace(filter.StoreId) &&
                !string.IsNullOrWhiteSpace(filter.StoreCityId),
                s => s.Store!.Address.CityId == Guid.Parse(filter.StoreCityId!))
            .WhereIf(string.IsNullOrWhiteSpace(filter.StoreId) &&
                !string.IsNullOrWhiteSpace(filter.StoreDistrictId),
                s => s.Store!.Address.DistrictId == Guid.Parse(filter.StoreDistrictId!))
            .WhereIf(filter.IsSent.HasValue && filter.IsSent == true, s => s.SendingDate.HasValue)
            .WhereIf(filter.IsSent.HasValue && filter.IsSent == false, s => !s.SendingDate.HasValue);
        
        if (withPagination)
            query = query
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize);
        
        return query;    
    }

    public static IQueryable<DeliveryItem> ApplyDeliveryItemFilter(
        this IQueryable<DeliveryItem> query,
        DeliveryItemFilter filter,
        bool withPagination = true
    )
    {
        query = query
            .WhereIf(!string.IsNullOrWhiteSpace(filter.DeliveryId), d => d.DeliveryId == Guid.Parse(filter.DeliveryId!))
            .WhereIf(!string.IsNullOrWhiteSpace(filter.SkuId), d => d.SkuId == Guid.Parse(filter.SkuId!))
            .WhereIf(filter.MinQuantity.HasValue, d => d.Quantity >= filter.MinQuantity)
            .WhereIf(filter.MaxQuantity.HasValue, d => d.Quantity <= filter.MaxQuantity);

        if (withPagination)
            query = query
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize);
        
        return query;
    }

    public static IQueryable<Delivery> ApplyDeliveryFilter(
        this IQueryable<Delivery> query,
        DeliveryFilter filter,
        bool withPagination = true
    )
    {
        query = query
            .WhereIf(!string.IsNullOrWhiteSpace(filter.WarehouseId), d => d.WarehouseId == Guid.Parse(filter.WarehouseId!))
            .WhereIf(string.IsNullOrWhiteSpace(filter.WarehouseId)
                && !string.IsNullOrWhiteSpace(filter.CityId),
                d => d.Warehouse!.Address.CityId == Guid.Parse(filter.CityId!))
            .WhereIf(string.IsNullOrWhiteSpace(filter.WarehouseId)
                && !string.IsNullOrWhiteSpace(filter.DistrictId),
                d => d.Warehouse!.Address.DistrictId == Guid.Parse(filter.DistrictId!))
            .WhereIf(filter.IsReceived.HasValue && filter.IsReceived == true, d => d.ReceivedAt != null)
            .WhereIf(filter.IsReceived.HasValue && filter.IsReceived == false, d => d.ReceivedAt == null);

        if (withPagination)
            query = query
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize);

        return query;
    }

    public static IQueryable<Store> ApplyStoreFilter(
        this IQueryable<Store> query,
        StoreFilter filter,
        bool withPagination = true
    )
    {
        query = query
            .WhereIf(!string.IsNullOrWhiteSpace(filter.CityId), s => s.Address.CityId == Guid.Parse(filter.CityId!))
            .WhereIf(!string.IsNullOrWhiteSpace(filter.DistrictId), s => s.Address.DistrictId == Guid.Parse(filter.DistrictId!));

        if (withPagination)
            query = query
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize);
        
        return query;
    }

    public static IQueryable<Warehouse> ApplyWarehouseFilter(
        this IQueryable<Warehouse> query,
        WarehouseFilter filter,
        bool withPagination = true 
    )
    {
        query = query
            .WhereIf(!string.IsNullOrWhiteSpace(filter.CityId), w => w.Address.CityId == Guid.Parse(filter.CityId!))
            .WhereIf(!string.IsNullOrWhiteSpace(filter.DistrictId), w => w.Address.DistrictId == Guid.Parse(filter.DistrictId!));
        
        if (withPagination)
            query = query
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize);
            
        return query;
    }
    
    private static IQueryable<T> WhereIf<T>(
        this IQueryable<T> query,
        bool condition,
        Expression<Func<T, bool>> predicate
    )
    {
        return condition ? query.Where(predicate) : query;
    }
}
