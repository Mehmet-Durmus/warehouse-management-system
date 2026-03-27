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
        if (filter.CreatedAfter.HasValue)
            query = query.Where(e => e.CreatedAt >= filter.CreatedAfter);
        if (filter.CreatedBefore.HasValue)
            query = query.Where(e => e.CreatedAt <= filter.CreatedBefore);
        if (filter.UpdatedAfter.HasValue)
            query = query.Where(e => e.UpdatedAt >= filter.UpdatedAfter);
        if (filter.UpdatedBefore.HasValue)
            query = query.Where(e => e.UpdatedAt <= filter.UpdatedBefore);

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