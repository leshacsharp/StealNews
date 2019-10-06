using System;
using System.Linq.Expressions;

namespace StealNews.Common.Helpers
{
    public static class PredicateBuilder
    {
        public static Expression<Func<TEntity, bool>> True<TEntity>()
        {
            return p => true;
        }

        public static Expression<Func<TEntity, bool>> False<TEntity>()
        {
            return p => false;
        }

        public static Expression<Func<TEntity, bool>> And<TEntity>(this Expression<Func<TEntity, bool>> first,
                                                                        Expression<Func<TEntity, bool>> second)
        {
            var parametrs = first.Parameters;
            var firstInvokedExp = Expression.Invoke(first, parametrs);
            var secondInvokedExp = Expression.Invoke(second, parametrs);

            return Expression.Lambda<Func<TEntity, bool>>(Expression.AndAlso(firstInvokedExp, secondInvokedExp), parametrs);
        }

        public static Expression<Func<TEntity, bool>> Or<TEntity>(this Expression<Func<TEntity, bool>> first,
                                                                        Expression<Func<TEntity, bool>> second)
        {
            var parametrs = first.Parameters;
            var firstInvokedExp = Expression.Invoke(first, parametrs);
            var secondInvokedExp = Expression.Invoke(second, parametrs);

            return Expression.Lambda<Func<TEntity, bool>>(Expression.OrElse(firstInvokedExp, secondInvokedExp), parametrs);
        }
    }
}
