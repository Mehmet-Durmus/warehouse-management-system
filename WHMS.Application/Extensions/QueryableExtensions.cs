using System.Linq.Expressions;
using WHMS.Application.Filters;
using WHMS.Domain.Entities;

namespace WHMS.Application.Extensions;

public static class QueryableExtensions
{
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
