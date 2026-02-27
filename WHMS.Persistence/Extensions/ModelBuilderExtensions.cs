using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using WHMS.Domain.Entities.Abstractions;

namespace WHMS.Persistence.Extensions;

public static class ModelBuilderExtensions
{
    public static void AddSoftDeleteQueryFilter(this ModelBuilder builder)
    {
        var softDeletableTypes = builder.Model.GetEntityTypes()
            .Where(t => typeof(ISoftDeletable).IsAssignableFrom(t.ClrType));

        foreach (var softDeletableType in softDeletableTypes)
            builder.Entity(softDeletableType.ClrType)
                .HasQueryFilter(CreateIsActiveFilter(softDeletableType.ClrType));
    }

    private static LambdaExpression CreateIsActiveFilter(Type type)
    {
        var parameter = Expression.Parameter(type, "e");
        var property = Expression.Property(parameter, nameof(ISoftDeletable.IsActive));
        return Expression.Lambda(property, parameter);
    }
}