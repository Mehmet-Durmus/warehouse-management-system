
using WHMS.Application.Common.Filtering.Filters;
using WHMS.Domain.Entities;

namespace WHMS.Application.Common.Filtering.Extensions;

public static class EmployeeFilterExtensions
{
    public static IQueryable<ApplicationUser> Apply(
        this IQueryable<ApplicationUser> query,
        EmployeeFilter filter,
        bool applyPagination = true
    )
    {
        return query
            .ApplyEmployeeFilter(filter)
            .ApplyCommonFilters(filter, applyPagination);
    }

    private static IQueryable<ApplicationUser> ApplyEmployeeFilter(
        this IQueryable<ApplicationUser> query,
        EmployeeFilter filter
    )
    {
        return query
            .WhereIf(!string.IsNullOrWhiteSpace(filter.WarehouseId),
                e => e.WarehouseId == Guid.Parse(filter.WarehouseId!))
            .WhereIf(string.IsNullOrWhiteSpace(filter.WarehouseId)
                && !string.IsNullOrWhiteSpace(filter.CityId),
                e => e.Warehouse!.Address.CityId == Guid.Parse(filter.CityId!))
            .WhereIf(string.IsNullOrWhiteSpace(filter.WarehouseId)
                && !string.IsNullOrWhiteSpace(filter.DistrictId),
                e => e.Warehouse!.Address.DistrictId == Guid.Parse(filter.DistrictId!))
            .WhereIf(string.IsNullOrWhiteSpace(filter.WarehouseId)
                && !string.IsNullOrWhiteSpace(filter.NeighborhoodId),
                e => e.Warehouse!.Address.NeighborhoodId == Guid.Parse(filter.NeighborhoodId!))
            .WhereIf(!string.IsNullOrWhiteSpace(filter.Name),
                e => e.FullName.Contains(filter.Name!))
            .WhereIf(filter.IsAssignedToWarehouse.HasValue && filter.IsAssignedToWarehouse == true,
                e => e.WarehouseId != null)
            .WhereIf(filter.IsAssignedToWarehouse.HasValue && filter.IsAssignedToWarehouse == false,
                e => e.WarehouseId == null);
    }
}