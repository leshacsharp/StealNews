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
            var parametr = first.Parameters[0];
            var firstInvokedExp = Expression.Invoke(first);
            var secondInvokedExp = Expression.Invoke(second);

            return Expression.Lambda<Func<TEntity, bool>>(Expression.And(firstInvokedExp, secondInvokedExp), parametr);
        }

        public static Expression<Func<TEntity, bool>> Or<TEntity>(this Expression<Func<TEntity, bool>> first,
                                                                        Expression<Func<TEntity, bool>> second)
        {
            var parametr = first.Parameters[0];
            var firstInvokedExp = Expression.Invoke(first);
            var secondInvokedExp = Expression.Invoke(second);

            return Expression.Lambda<Func<TEntity, bool>>(Expression.Or(firstInvokedExp, secondInvokedExp), parametr);
        }
    }
}
