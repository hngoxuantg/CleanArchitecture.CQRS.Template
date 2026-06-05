using Microsoft.EntityFrameworkCore;
using Project.Domain.Interfaces.IRepositories.IBaseRepositories;
using Project.Infrastructure.Data.Contexts;
using System.Linq.Expressions;

namespace Project.Infrastructure.Data.Repositories.BaseRepositories
{
    public abstract class BaseRepository<T> : IBaseRepository<T>
        where T : class
    {
        protected readonly ApplicationDbContext _dbContext;

        protected BaseRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        private static IQueryable<T> ApplyQuery(
            IQueryable<T> source,
            RepositoryQuery<T>? query)
        {
            if (query == null) return source;

            if (query.Filter != null)
                source = source.Where(query.Filter);

            if (query.Includes != null)
                source = query.Includes.Compile()(source);

            if (query.Order != null)
                source = query.Order.Compile()(source);

            return source;
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync(
            RepositoryQuery<T>? query = null,
            CancellationToken ct = default)
        {
            IQueryable<T> q = ApplyQuery(_dbContext.Set<T>().AsNoTracking(), query);
            return await q.ToListAsync(ct);
        }

        public virtual async Task<IEnumerable<TResult>> GetAllAsync<TResult>(
            Expression<Func<T, TResult>> selector,
            RepositoryQuery<T>? query = null,
            CancellationToken ct = default)
        {
            IQueryable<T> q = ApplyQuery(_dbContext.Set<T>().AsNoTracking(), query);
            return await q.Select(selector).ToListAsync(ct);
        }

        public virtual async Task<T?> GetByIdAsync<TId>(
            TId id,
            CancellationToken ct = default)
        {
            return await _dbContext.Set<T>().FindAsync(new object?[] { id }, ct);
        }

        public virtual async Task<T?> GetByIdAsync<TId>(
            TId id,
            RepositoryQuery<T>? query = null,
            CancellationToken ct = default)
        {
            if (query?.Includes == null)
                return await _dbContext.Set<T>().FindAsync(new object?[] { id }, ct);

            string? keyName = _dbContext.Model
                .FindEntityType(typeof(T))?
                .FindPrimaryKey()?
                .Properties
                .FirstOrDefault()?
                .Name;

            if (keyName == null)
                throw new InvalidOperationException(
                    $"Cannot determine primary key for entity {typeof(T).Name}.");

            ParameterExpression parameter = Expression.Parameter(typeof(T), "x");

            MemberExpression property = Expression.Property(parameter, keyName);
            ConstantExpression constant = Expression.Constant(id);

            BinaryExpression equality = Expression.Equal(property, constant);

            Expression<Func<T, bool>> lambda = Expression.Lambda<Func<T, bool>>(equality, parameter);

            IQueryable<T> q = ApplyQuery(_dbContext.Set<T>(), query);
            return await q.Where(lambda).FirstOrDefaultAsync(ct);
        }

        public virtual async Task<T?> GetOneAsync(
            RepositoryQuery<T> query,
            CancellationToken ct = default)
        {
            IQueryable<T> q = ApplyQuery(_dbContext.Set<T>(), query);
            return await q.FirstOrDefaultAsync(ct);
        }

        public virtual async Task<TResult?> GetOneAsync<TResult>(
            Expression<Func<T, TResult>> selector,
            RepositoryQuery<T> query,
            CancellationToken ct = default)
        {
            IQueryable<T> q = ApplyQuery(_dbContext.Set<T>(), query);
            return await q.Select(selector).FirstOrDefaultAsync(ct);
        }

        public async Task<T?> GetOneUntrackedAsync(
            RepositoryQuery<T> query,
            CancellationToken ct = default)
        {
            IQueryable<T> q = ApplyQuery(_dbContext.Set<T>().AsNoTracking(), query);
            return await q.FirstOrDefaultAsync(ct);
        }

        public async Task<TResult?> GetOneUntrackedAsync<TResult>(
            Expression<Func<T, TResult>> selector,
            RepositoryQuery<T> query,
            CancellationToken ct = default)
        {
            IQueryable<T> q = ApplyQuery(_dbContext.Set<T>().AsNoTracking(), query);
            return await q.Select(selector).FirstOrDefaultAsync(ct);
        }

        public virtual async Task<(IEnumerable<T> items, int totalCount)> GetPagedAsync(
            RepositoryQuery<T>? query = null,
            int pageNumber = 1,
            int pageSize = 12,
            CancellationToken ct = default)
        {
            IQueryable<T> countQuery = _dbContext.Set<T>().AsNoTracking();
            if (query?.Filter != null)
                countQuery = countQuery.Where(query.Filter);

            int count = await countQuery.CountAsync(ct);

            IQueryable<T> q = ApplyQuery(_dbContext.Set<T>().AsNoTracking(), query);
            q = q.Skip((pageNumber - 1) * pageSize).Take(pageSize);

            return (await q.ToListAsync(ct), count);
        }

        public virtual async Task<(IEnumerable<TResult> items, int totalCount)> GetPagedAsync<TResult>(
            Expression<Func<T, TResult>> selector,
            RepositoryQuery<T>? query = null,
            int pageNumber = 1,
            int pageSize = 12,
            CancellationToken ct = default)
        {
            IQueryable<T> countQuery = _dbContext.Set<T>().AsNoTracking();
            if (query?.Filter != null)
                countQuery = countQuery.Where(query.Filter);

            int count = await countQuery.CountAsync(ct);

            IQueryable<T> q = ApplyQuery(_dbContext.Set<T>().AsNoTracking(), query);
            q = q.Skip((pageNumber - 1) * pageSize).Take(pageSize);

            return (await q.Select(selector).ToListAsync(ct), count);
        }

        public virtual async Task<int> GetCountAsync(
            RepositoryQuery<T>? query = null,
            CancellationToken ct = default)
        {
            IQueryable<T> q = _dbContext.Set<T>().AsNoTracking();
            if (query?.Filter != null)
                q = q.Where(query.Filter);

            return await q.CountAsync(ct);
        }

        public virtual async Task<bool> IsExistsAsync<TValue>(
            string key,
            TValue value,
            CancellationToken ct = default)
        {
            Expression<Func<T, bool>> lambda = BuildEqualityLambda(key, value);
            return await _dbContext.Set<T>().AnyAsync(lambda, ct);
        }

        public virtual async Task<bool> IsExistsForUpdateAsync<TId, TValue>(
            TId id,
            string key,
            TValue value,
            CancellationToken ct = default)
        {
            string? idName = _dbContext.Model
                .FindEntityType(typeof(T))?
                .FindPrimaryKey()?
                .Properties
                .FirstOrDefault()?
                .Name ?? "Id";

            ParameterExpression parameter = Expression.Parameter(typeof(T), "x");

            MemberExpression property = Expression.Property(parameter, key);
            ConstantExpression constant = Expression.Constant(value);

            BinaryExpression equality = Expression.Equal(property, constant);

            MemberExpression idProperty = Expression.Property(parameter, idName);

            BinaryExpression idEquality = Expression.NotEqual(idProperty, Expression.Constant(id));

            BinaryExpression combined = Expression.AndAlso(equality, idEquality);
            Expression<Func<T, bool>> lambda = Expression.Lambda<Func<T, bool>>(combined, parameter);

            return await _dbContext.Set<T>().AnyAsync(lambda, ct);
        }

        public virtual async Task<T> CreateAsync(T model, CancellationToken ct = default)
        {
            _dbContext.Set<T>().Add(model);
            await _dbContext.SaveChangesAsync(ct);
            return model;
        }

        public virtual async Task<IEnumerable<T>> CreateRangeAsync(
            IEnumerable<T> models,
            CancellationToken ct = default)
        {
            _dbContext.Set<T>().AddRange(models);
            await _dbContext.SaveChangesAsync(ct);
            return models;
        }

        public virtual async Task<T> UpdateAsync(T model, CancellationToken ct = default)
        {
            _dbContext.Set<T>().Update(model);
            await _dbContext.SaveChangesAsync(ct);
            return model;
        }

        public virtual async Task<IEnumerable<T>> UpdateRangeAsync(
            IEnumerable<T> models,
            CancellationToken ct = default)
        {
            _dbContext.Set<T>().UpdateRange(models);
            await _dbContext.SaveChangesAsync(ct);
            return models;
        }

        public virtual async Task DeleteAsync(T model, CancellationToken ct = default)
        {
            _dbContext.Set<T>().Remove(model);
            await _dbContext.SaveChangesAsync(ct);
        }

        public virtual async Task DeleteRangeAsync(
            IEnumerable<T> models,
            CancellationToken ct = default)
        {
            _dbContext.Set<T>().RemoveRange(models);
            await _dbContext.SaveChangesAsync(ct);
        }

        public virtual async Task SaveChangeAsync(CancellationToken ct = default)
        {
            await _dbContext.SaveChangesAsync(ct);
        }

        public virtual void AddEntity(T entity) => _dbContext.Set<T>().Add(entity);
        public virtual void AddRangeEntity(IEnumerable<T> entities) => _dbContext.Set<T>().AddRange(entities);
        public virtual void UpdateEntity(T entity) => _dbContext.Set<T>().Update(entity);
        public virtual void UpdateRangeEntity(IEnumerable<T> entities) => _dbContext.Set<T>().UpdateRange(entities);
        public virtual void DeleteEntity(T entity) => _dbContext.Set<T>().Remove(entity);
        public virtual void DeleteRangeEntity(IEnumerable<T> entities) => _dbContext.Set<T>().RemoveRange(entities);


        private static Expression<Func<T, bool>> BuildEqualityLambda<TValue>(
            string propertyName,
            TValue value)
        {
            ParameterExpression parameter = Expression.Parameter(typeof(T), "x");
            MemberExpression property = Expression.Property(parameter, propertyName);
            ConstantExpression constant = Expression.Constant(value);
            BinaryExpression equality = Expression.Equal(property, constant);

            return Expression.Lambda<Func<T, bool>>(equality, parameter);
        }
    }
}