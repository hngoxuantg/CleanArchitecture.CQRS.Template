using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Project.Domain.Interfaces.IRepositories.IBaseRepositories
{
    public class RepositoryQuery<T> where T : class
    {
        private Expression<Func<T, bool>>? _filter;
        private Expression<Func<IQueryable<T>, IOrderedQueryable<T>>>? _orderBy;
        private Expression<Func<IQueryable<T>, IQueryable<T>>>? _include;

        private RepositoryQuery() { }

        public static RepositoryQuery<T> For() => new();

        public RepositoryQuery<T> Where(Expression<Func<T, bool>> filter)
        {
            _filter = filter;
            return this;
        }

        public RepositoryQuery<T> Include<TProperty>(
            Expression<Func<T, TProperty>> navigationProperty)
        {
            _include = q => q.Include(navigationProperty);
            return this;
        }

        public RepositoryQuery<T> ThenInclude<TPreviousProperty, TProperty>(
            Expression<Func<TPreviousProperty, TProperty>> navigationProperty)
        {
            if (_include == null)
                throw new InvalidOperationException(
                    "Must call Include before ThenInclude.");

            Expression<Func<IQueryable<T>, IQueryable<T>>> previous = _include;
            _include = q =>
                ((IIncludableQueryable<T, TPreviousProperty>)previous.Compile()(q))
                .ThenInclude(navigationProperty);

            return this;
        }

        public RepositoryQuery<T> ThenInclude<TPreviousProperty, TProperty>(
            Expression<Func<IEnumerable<TPreviousProperty>, TProperty>> navigationProperty)
        {
            if (_include == null)
                throw new InvalidOperationException(
                    "Must call Include before ThenInclude.");

            Expression<Func<IQueryable<T>, IQueryable<T>>> previous = _include;
            _include = q =>
                ((IIncludableQueryable<T, IEnumerable<TPreviousProperty>>)previous.Compile()(q))
                .ThenInclude(navigationProperty);

            return this;
        }

        public RepositoryQuery<T> OrderBy<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            _orderBy = q => q.OrderBy(keySelector);
            return this;
        }

        public RepositoryQuery<T> OrderByDescending<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            _orderBy = q => q.OrderByDescending(keySelector);
            return this;
        }

        public RepositoryQuery<T> ThenBy<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            if (_orderBy == null)
                throw new InvalidOperationException(
                    "Must call OrderBy or OrderByDescending before ThenBy.");

            Expression<Func<IQueryable<T>, IOrderedQueryable<T>>> previous = _orderBy;
            _orderBy = q =>
                ((IOrderedQueryable<T>)previous.Compile()(q)).ThenBy(keySelector);

            return this;
        }

        public RepositoryQuery<T> ThenByDescending<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            if (_orderBy == null)
                throw new InvalidOperationException(
                    "Must call OrderBy or OrderByDescending before ThenByDescending.");

            Expression<Func<IQueryable<T>, IOrderedQueryable<T>>> previous = _orderBy;
            _orderBy = q =>
                ((IOrderedQueryable<T>)previous.Compile()(q)).ThenByDescending(keySelector);

            return this;
        }

        public Expression<Func<T, bool>>? Filter => _filter;
        public Expression<Func<IQueryable<T>, IOrderedQueryable<T>>>? Order => _orderBy;
        public Expression<Func<IQueryable<T>, IQueryable<T>>>? Includes => _include;
    }
}