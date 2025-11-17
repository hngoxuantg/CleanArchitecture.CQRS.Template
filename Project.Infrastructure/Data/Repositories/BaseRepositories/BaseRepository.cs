using Microsoft.EntityFrameworkCore;
using Project.Domain.Interfaces.IRepositories.IBaseRepositories;
using Project.Infrastructure.Data.Contexts;
using System.Linq.Expressions;

namespace Project.Infrastructure.Data.Repositories.BaseRepositories
{
    public abstract class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _dbContext;
        public BaseRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<TResult>> GetAllAsync<TResult>(
            Expression<Func<T, bool>>? filter = null,
            Expression<Func<IQueryable<T>, IOrderedQueryable<T>>>? orderBy = null,
            Expression<Func<T, TResult>>? selector = null,
            Expression<Func<IQueryable<T>, IQueryable<T>>>? include = null,
            CancellationToken cancellation = default)
        {
            IQueryable<T> query = _dbContext.Set<T>().AsNoTracking();
            if (filter != null)
                query = query.Where(filter);

            if (include != null)
                query = include.Compile()(query);

            if (orderBy != null)
                query = orderBy.Compile()(query);

            if (selector != null)
                return await query.Select(selector).ToListAsync(cancellation);
            else
                return await query.Cast<TResult>().ToListAsync(cancellation);
        }

        public async Task<T?> GetByIdAsync<Tid>(Tid id, CancellationToken cancellation = default)
        {
            return await _dbContext.Set<T>().FindAsync(id, cancellation);
        }

        public async Task<T?> GetByIdAsync<Tid>(Tid id,
            Expression<Func<IQueryable<T>, IQueryable<T>>>? include = null,
            CancellationToken cancellation = default)
        {
            if (include == null)
                return await _dbContext.Set<T>().FindAsync(id, cancellation);
            string? keyProperty = _dbContext.Model
                .FindEntityType(typeof(T))?
                .FindPrimaryKey()?
                .Properties?
                .FirstOrDefault()?
                .Name;

            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(parameter, keyProperty);
            var constant = Expression.Constant(id);
            var equality = Expression.Equal(property, constant);
            var lamda = Expression.Lambda<Func<T, bool>>(equality, parameter);

            IQueryable<T> query = _dbContext.Set<T>();

            return await include.Compile()(query).Where(lamda).FirstOrDefaultAsync(cancellation);
        }

        public async Task<TResult?> GetOneUntrackedAsync<TResult>(
            Expression<Func<T, bool>>? filter = null,
            Expression<Func<IQueryable<T>, IOrderedQueryable<T>>>? orderBy = null,
            Expression<Func<T, TResult>>? selector = null,
            Expression<Func<IQueryable<T>, IQueryable<T>>>? include = null,
            CancellationToken cancellation = default)
        {
            IQueryable<T> query = _dbContext.Set<T>().AsNoTracking();
            if (filter != null)
                query = query.Where(filter);

            if (include != null)
                query = include.Compile()(query);

            if (orderBy != null)
                query = orderBy.Compile()(query);

            if (selector != null)
                return await query.Select(selector).FirstOrDefaultAsync(cancellation);
            else
                return await query.Cast<TResult>().FirstOrDefaultAsync(cancellation);
        }

        public async Task<TResult?> GetOneAsync<TResult>(
            Expression<Func<T, bool>>? filter = null,
            Expression<Func<IQueryable<T>, IOrderedQueryable<T>>>? orderBy = null,
            Expression<Func<T, TResult>>? selector = null,
            Expression<Func<IQueryable<T>, IQueryable<T>>>? include = null,
            CancellationToken cancellation = default)
        {
            IQueryable<T> query = _dbContext.Set<T>();
            if (filter != null)
                query = query.Where(filter);

            if (include != null)
                query = include.Compile()(query);

            if (orderBy != null)
                query = orderBy.Compile()(query);

            if (selector != null)
                return await query.Select(selector).FirstOrDefaultAsync(cancellation);
            else
                return await query.Cast<TResult>().FirstOrDefaultAsync(cancellation);
        }

        public virtual async Task<(IEnumerable<T>, int totalCount)> GetPagedAsync(
            Expression<Func<T, bool>>? filter = null,
            Expression<Func<IQueryable<T>, IOrderedQueryable<T>>>? orderBy = null,
            Expression<Func<IQueryable<T>, IQueryable<T>>>? include = null,
            int pageNumber = 1,
            int pageSize = 12,
            CancellationToken cancellationToken = default)
        {
            IQueryable<T> query = _dbContext.Set<T>().AsNoTracking();

            if (filter != null)
                query = query.Where(filter);

            int count = await query.CountAsync(cancellationToken);

            if (include != null)
                query = include.Compile()(query);

            if (orderBy != null)
                query = orderBy.Compile()(query);

            query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            return (await query.ToListAsync(cancellationToken), count);
        }

        public virtual async Task<(IEnumerable<TResult>, int totalCount)> GetPagedAsync<TResult>(
            Expression<Func<T, TResult>> selector,
            Expression<Func<T, bool>>? filter = null,
            Expression<Func<IQueryable<T>, IOrderedQueryable<T>>>? orderBy = null,
            Expression<Func<IQueryable<T>, IQueryable<T>>>? include = null,
            int pageNumber = 1,
            int pageSize = 12,
            CancellationToken cancellationToken = default)
        {
            IQueryable<T> query = _dbContext.Set<T>().AsNoTracking();

            if (filter != null)
                query = query.Where(filter);

            int count = await query.CountAsync(cancellationToken);

            if (include != null)
                query = include.Compile()(query);

            if (orderBy != null)
                query = orderBy.Compile()(query);

            query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);

            var result = await query.Select(selector).ToListAsync(cancellationToken);

            return (result, count);
        }

        public async Task<int> GetCountAsync(
            Expression<Func<T, bool>>? filters = null,
            CancellationToken cancellation = default)
        {
            IQueryable<T> query = _dbContext.Set<T>().AsNoTracking();

            if (filters != null)
                query = query.Where(filters);

            return await query.CountAsync(cancellation);
        }

        public virtual async Task<T> CreateAsync(T model, CancellationToken cancellation = default)
        {
            _dbContext.Set<T>().Add(model);
            await _dbContext.SaveChangesAsync(cancellation);
            return model;
        }

        public virtual async Task<IEnumerable<T>> CreateRangeAsync(IEnumerable<T> models, CancellationToken cancellation = default)
        {
            _dbContext.Set<T>().AddRange(models);
            await _dbContext.SaveChangesAsync(cancellation);
            return models;
        }

        public async Task<T> UpdateAsync(T model, CancellationToken cancellation = default)
        {
            _dbContext.Set<T>().Update(model);
            await _dbContext.SaveChangesAsync(cancellation);
            return model;
        }

        public async Task<IEnumerable<T>> UpdateRangeAsync(IEnumerable<T> models, CancellationToken cancellation = default)
        {
            _dbContext.Set<T>().UpdateRange(models);
            await _dbContext.SaveChangesAsync(cancellation);
            return models;
        }

        public async Task DeleteAsync(T model, CancellationToken cancellation = default)
        {
            _dbContext.Set<T>().Remove(model);
            await _dbContext.SaveChangesAsync(cancellation);
        }
        public async Task DeleteRangeAsync(IEnumerable<T> models, CancellationToken cancellation = default)
        {
            _dbContext.Set<T>().RemoveRange(models);
            await _dbContext.SaveChangesAsync(cancellation);
        }

        public async Task SaveChangeAsync(CancellationToken cancellation = default)
        {
            await _dbContext.SaveChangesAsync(cancellation);
        }

        public async Task<bool> IsExistsAsync<TValue>(string key, TValue value, CancellationToken cancellation = default)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(parameter, key);
            var constant = Expression.Constant(value);
            var equality = Expression.Equal(property, constant);
            var lamda = Expression.Lambda<Func<T, bool>>(equality, parameter);

            return await _dbContext.Set<T>().AnyAsync(lamda, cancellation);
        }

        public async Task<bool> IsExistsForUpdateAsync<Tid, TValue>(
            Tid id,
            string key,
            TValue value,
            CancellationToken cancellation = default)
        {
            string? idPrimary = _dbContext.Model
                .FindEntityType(typeof(T))?
                .FindPrimaryKey()?
                .Properties
                .FirstOrDefault()?
                .Name;

            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(parameter, key);
            var constant = Expression.Constant(value);
            var equality = Expression.Equal(property, constant);

            var idProperty = Expression.Property(parameter, idPrimary ?? "Id");
            var idEquality = Expression.NotEqual(idProperty, Expression.Constant(id));

            var combinedExpression = Expression.AndAlso(equality, idEquality);
            var lamda = Expression.Lambda<Func<T, bool>>(combinedExpression, parameter);

            return await _dbContext.Set<T>().AnyAsync(lamda, cancellation);
        }

        public virtual void AddEntity(T entity)
        {
            _dbContext.Set<T>().Add(entity);
        }

        public virtual void AddRangeEntity(IEnumerable<T> entities)
        {
            _dbContext.Set<T>().AddRange(entities);
        }

        public virtual void UpdateEntity(T entity)
        {
            _dbContext.Set<T>().Update(entity);
        }

        public virtual void UpdateRangeEntity(IEnumerable<T> entities)
        {
            _dbContext.Set<T>().UpdateRange(entities);
        }

        public virtual void DeleteEntity(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
        }

        public virtual void DeleteRangeEntity(IEnumerable<T> entities)
        {
            _dbContext.Set<T>().RemoveRange(entities);
        }
    }
}