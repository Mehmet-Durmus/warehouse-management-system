using System.Globalization;
using WHMS.Application.Common.Filtering.Filters;
using WHMS.Domain.Entities;

namespace WHMS.Application.Common.Filtering.Extensions;

public static class WasteRecordFilterExtensions
{
    public static IQueryable<WasteRecord> Apply(
        this IQueryable<WasteRecord> query,
        WasteRecordFilter filter,
        bool applyPagination = true
    )
    {
        return query
            .ApplyWasteRecordFilter(filter)
            .ApplyCommonFilters(filter, applyPagination);
    }

    private static IQueryable<WasteRecord> ApplyWasteRecordFilter(
        this IQueryable<WasteRecord> query,
        WasteRecordFilter filter
    )
    {
        return query
            .WhereIf(!string.IsNullOrWhiteSpace(filter.WarehouseId),
                w => w.WarehouseId == Guid.Parse(filter.WarehouseId!))
            
            .WhereIf(!string.IsNullOrWhiteSpace(filter.SkuId),
                w => w.SkuId == Guid.Parse(filter.SkuId!))
            
            .WhereIf(!string.IsNullOrWhiteSpace(filter.CreatedById),
                w => w.CreatedById == Guid.Parse(filter.CreatedById!))

            .WhereIf(!string.IsNullOrWhiteSpace(filter.Description),
                w => w.Description != null && (w.Description.ToUpper(CultureInfo.GetCultureInfo("tr-TR"))
                .Contains(filter.Description!.ToUpper(CultureInfo.GetCultureInfo("tr-TR")))))
            
            .WhereIf(filter.MaxQuantity.HasValue,
                w => w.Quantity <= filter.MaxQuantity)
            
            .WhereIf(filter.MinQuantity.HasValue,
                w => w.Quantity >= filter.MinQuantity);
    }
}