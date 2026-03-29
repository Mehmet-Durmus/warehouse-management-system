using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using WHMS.Domain.Entities.Abstractions;

namespace WHMS.Application.Common.Filtering.Extensions;

public static class QueryFilterExtensions
{
    public static IQueryable<T> ApplyCommonFilters<T>(
        this IQueryable<T> query,
        QueryFilter filter,
        bool applyPagination
    ) where T : IAuditable
    {
        query = query
            .WhereIf(filter.CreatedAfter.HasValue,
                e => e.CreatedAt >= filter.CreatedAfter)
            .WhereIf(filter.CreatedBefore.HasValue,
                e => e.CreatedAt <= filter.CreatedBefore)
            .WhereIf(filter.UpdatedAfter.HasValue,
                e => e.CreatedAt >= filter.UpdatedAfter)
            .WhereIf(filter.UpdatedBefore.HasValue,
                e => e.CreatedAt <= filter.UpdatedBefore);
                
        if (applyPagination)
            query = query
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize);
        
        return query;
    }

    public static IQueryable<T> WhereIf<T>(
        this IQueryable<T> query,
        bool condition,
        Expression<Func<T, bool>> predicate
    )
    { return condition ? query.Where(predicate) : query; }
}