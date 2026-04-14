using BussinesMS.Aplicacion.DTOs.Plantillas;
using System.Linq.Expressions;
using System.Reflection;

namespace BussinesMS.Aplicacion.Comun;

public static class QueryableExtensions
{
    public static (IQueryable<TEntity> Query, int TotalCount) ApplyFilters<TEntity>(
        this IQueryable<TEntity> query,
        GenericPaginationQueryDto paginationQuery,
        bool skipSorting = false)
        where TEntity : class
    {
        query = query.ApplyFieldFilter(paginationQuery);
        var totalCount = query.Count();

        if (!skipSorting)
        {
            query = query.ApplySorting(paginationQuery);
        }

        query = query.ApplyPagination(paginationQuery);

        return (query, totalCount);
    }

    public static IQueryable<TEntity> ApplyFieldFilter<TEntity>(
        this IQueryable<TEntity> query,
        GenericPaginationQueryDto paginationQuery)
        where TEntity : class
    {
        if (string.IsNullOrWhiteSpace(paginationQuery.FieldName) ||
            string.IsNullOrWhiteSpace(paginationQuery.FieldValue))
        {
            return query;
        }

        var fieldNamePascalCase = char.ToUpper(paginationQuery.FieldName[0]) +
                                 paginationQuery.FieldName.Substring(1).ToLower();
        var fieldValueLower = paginationQuery.FieldValue.ToLower();

        var property = typeof(TEntity).GetProperty(fieldNamePascalCase,
            BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

        if (property == null)
        {
            return query;
        }

        var parameter = Expression.Parameter(typeof(TEntity), "w");
        var propertyAccess = Expression.Property(parameter, property);

        Expression propertyAsString;
        if (property.PropertyType != typeof(string))
        {
            var toStringMethod = typeof(object).GetMethod(nameof(object.ToString), Type.EmptyTypes);
            propertyAsString = Expression.Call(propertyAccess, toStringMethod!);
        }
        else
        {
            propertyAsString = propertyAccess;
        }

        var toLowerMethod = typeof(string).GetMethod(nameof(string.ToLower), Type.EmptyTypes);
        var propertyToLower = Expression.Call(propertyAsString, toLowerMethod!);
        var constant = Expression.Constant(fieldValueLower, typeof(string));
        var containsMethod = typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string) });
        var containsExpression = Expression.Call(propertyToLower, containsMethod!, constant);
        var lambda = Expression.Lambda<Func<TEntity, bool>>(containsExpression, parameter);

        return query.Where(lambda);
    }

    public static IQueryable<TEntity> ApplySorting<TEntity>(
        this IQueryable<TEntity> query,
        GenericPaginationQueryDto paginationQuery)
        where TEntity : class
    {
        if (string.IsNullOrWhiteSpace(paginationQuery.SortBy))
        {
            var defaultProperty = typeof(TEntity).GetProperty("Id");
            if (defaultProperty != null)
            {
                var defaultParameter = Expression.Parameter(typeof(TEntity), "x");
                var defaultPropertyAccess = Expression.Property(defaultParameter, defaultProperty);
                var defaultOrderByExp = Expression.Lambda<Func<TEntity, object>>(
                    Expression.Convert(defaultPropertyAccess, typeof(object)), defaultParameter);
                return query.OrderBy(defaultOrderByExp);
            }
            return query;
        }

        var sortBy = paginationQuery.SortBy.Trim();
        var propertyName = char.ToUpper(sortBy[0]) + sortBy.Substring(1);
        
        var property = typeof(TEntity).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
        if (property == null)
        {
            return query;
        }

        var parameter = Expression.Parameter(typeof(TEntity), "x");
        var propertyAccess = Expression.Property(parameter, property);
        var orderByExp = Expression.Lambda<Func<TEntity, object>>(
            Expression.Convert(propertyAccess, typeof(object)), parameter);

        if (!string.IsNullOrWhiteSpace(paginationQuery.SortDirection) &&
            paginationQuery.SortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase))
        {
            return query.OrderByDescending(orderByExp);
        }

        return query.OrderBy(orderByExp);
    }

    public static IQueryable<TEntity> ApplyPagination<TEntity>(
        this IQueryable<TEntity> query,
        GenericPaginationQueryDto paginationQuery)
        where TEntity : class
    {
        if (paginationQuery.GetIsPagedValue())
        {
            query = query
                .Skip((paginationQuery.GetPageValue() - 1) * paginationQuery.GetPageSizeValue())
                .Take(paginationQuery.GetPageSizeValue());
        }

        return query;
    }
}