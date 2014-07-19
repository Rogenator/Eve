using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Eve.Business.Services
{
    public static class Extensions
    {
        public static IQueryable<T> OrderBy<T>(this IQueryable<T> source, string orderByProperty, bool desc = false)
        {
            string command = desc ? "OrderByDescending" : "OrderBy";
            var type = typeof(T);
            var property = type.GetProperty(orderByProperty);
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);
            var resultExpression = Expression.Call(typeof(Queryable), command, new Type[] { type, property.PropertyType }, source.Expression, Expression.Quote(orderByExpression));
            return source.Provider.CreateQuery<T>(resultExpression);
        }

        public static IEnumerable<T> OrderBy<T>(this IEnumerable<T> source, string orderByProperty, bool desc = false)
        {
            return source.AsQueryable().OrderBy(orderByProperty, desc).AsEnumerable();
        }

        public static IQueryable OrderBy(this IQueryable source, string orderByProperty, bool desc = false)
        {
            string command = desc ? "OrderByDescending" : "OrderBy";
            var type = source.ElementType;
            var property = type.GetProperty(orderByProperty);
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);
            var resultExpression = Expression.Call(typeof(Queryable), command, new Type[] { type, property.PropertyType }, source.Expression, Expression.Quote(orderByExpression));
            return source.Provider.CreateQuery(resultExpression);
        }
    }
}
