using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Reflection;

namespace CoolMeetingWeb.Biz
{
    public class EFSortHelper
    {
        public static IQueryable<T> PagedSort<T>(IQueryable<T> source, string sortFieldName, bool descending)
        {
            Type entityType = typeof(T);
            ParameterExpression pe = Expression.Parameter(entityType);
            PropertyInfo pi = entityType.GetProperty(sortFieldName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
            Type[] types = new Type[2];
            types[0] = entityType;
            types[1] = pi.PropertyType;
            string sortDirection = descending ? "OrderByDescending" : "OrderBy";

            Expression expr = Expression.Call(
                typeof(Queryable), sortDirection, types, source.Expression,
                Expression.Lambda(Expression.Property(pe, sortFieldName), pe));

            return source.Provider.CreateQuery<T>(expr);
        }
    }
}