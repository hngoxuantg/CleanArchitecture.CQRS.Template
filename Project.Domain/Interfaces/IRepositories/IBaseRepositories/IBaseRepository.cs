using System.Linq.Expressions;

namespace Project.Domain.Interfaces.IRepositories.IBaseRepositories
{
    public interface IBaseRepository<T> where T : class
    {
        Task<IEnumerable<TResult>> GetAllAsync<TResult>(
            Expression<Func<T, bool>>? filter = null,
            Expression<Func<IQueryable<T>, IOrderedQueryable<T>>>? orderBy = null,
            Expression<Func<T, TResult>>? selector = null,
            Expression<Func<IQueryable<T>, IQueryable<T>>>? include = null,
            CancellationToken cancellation = default);

        Task<T?> GetByIdAsync<Tid>(Tid id, CancellationToken cancellation = default);

        Task<T?> GetByIdAsync<Tid>(
            Tid id,
            Expression<Func<IQueryable<T>, IQueryable<T>>>? include = null,
            CancellationToken cancellation = default);

        Task<TResult?> GetOneUntrackedAsync<TResult>(
            Expression<Func<T, bool>>? filter = null,
            Expression<Func<IQueryable<T>, IOrderedQueryable<T>>>? orderBy = null,
            Expression<Func<T, TResult>>? selector = null,
            Expression<Func<IQueryable<T>, IQueryable<T>>>? include = null,
            CancellationToken cancellation = default);

        Task<TResult?> GetOneAsync<TResult>(
            Expression<Func<T, bool>>? filter = null,
            Expression<Func<IQueryable<T>, IOrderedQueryable<T>>>? orderBy = null,
            Expression<Func<T, TResult>>? selector = null,
            Expression<Func<IQueryable<T>, IQueryable<T>>>? include = null,
            CancellationToken cancellation = default);

        Task<(IEnumerable<T>, int totalCount)> GetPagedAsync(
            Expression<Func<T, bool>>? filter = null,
            Expression<Func<IQueryable<T>, IOrderedQueryable<T>>>? orderBy = null,
            Expression<Func<IQueryable<T>, IQueryable<T>>>? include = null,
            int pageNumber = 1,
            int pageSize = 12,
            CancellationToken cancellationToken = default);

        Task<(IEnumerable<TResult>, int totalCount)> GetPagedAsync<TResult>(
            Expression<Func<T, TResult>> selector,
            Expression<Func<T, bool>>? filter = null,
            Expression<Func<IQueryable<T>, IOrderedQueryable<T>>>? orderBy = null,
            Expression<Func<IQueryable<T>, IQueryable<T>>>? include = null,
            int pageNumber = 1,
            int pageSize = 12,
            CancellationToken cancellationToken = default);

        Task<int> GetCountAsync(Expression<Func<T, bool>>? filters = null, CancellationToken cancellation = default);

        Task<T> CreateAsync(T model, CancellationToken cancellation = default);

        Task<IEnumerable<T>> CreateRangeAsync(IEnumerable<T> models, CancellationToken cancellation = default);

        Task<T> UpdateAsync(T model, CancellationToken cancellation = default);

        Task<IEnumerable<T>> UpdateRangeAsync(IEnumerable<T> models, CancellationToken cancellation = default);

        Task DeleteAsync(T model, CancellationToken cancellation = default);

        Task DeleteRangeAsync(IEnumerable<T> models, CancellationToken cancellation = default);

        Task SaveChangeAsync(CancellationToken cancellation = default);

        Task<bool> IsExistsAsync<TValue>(string key, TValue value, CancellationToken cancellation = default);

        Task<bool> IsExistsForUpdateAsync<Tid, TValue>(
            Tid id,
            string key,
            TValue value,
            CancellationToken cancellation = default);

        void AddEntity(T entity);

        void AddRangeEntity(IEnumerable<T> entities);

        void UpdateEntity(T entity);

        void UpdateRangeEntity(IEnumerable<T> entities);

        void DeleteEntity(T entity);

        void DeleteRangeEntity(IEnumerable<T> entities);
    }
}
