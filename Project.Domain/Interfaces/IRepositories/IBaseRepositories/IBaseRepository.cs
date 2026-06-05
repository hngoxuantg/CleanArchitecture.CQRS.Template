using System.Linq.Expressions;

namespace Project.Domain.Interfaces.IRepositories.IBaseRepositories
{
    public interface IBaseRepository<T>
        where T : class
    {
        Task<IEnumerable<T>> GetAllAsync(
            RepositoryQuery<T>? query = null,
            CancellationToken ct = default);

        Task<IEnumerable<TResult>> GetAllAsync<TResult>(
            Expression<Func<T, TResult>> selector,
            RepositoryQuery<T>? query = null,
            CancellationToken ct = default);

        Task<T?> GetByIdAsync<TId>(
            TId id,
            CancellationToken ct = default);

        Task<T?> GetByIdAsync<TId>(
            TId id,
            RepositoryQuery<T>? query = null,
            CancellationToken ct = default);

        Task<T?> GetOneAsync(
            RepositoryQuery<T> query,
            CancellationToken ct = default);

        Task<TResult?> GetOneAsync<TResult>(
            Expression<Func<T, TResult>> selector,
            RepositoryQuery<T> query,
            CancellationToken ct = default);

        Task<TResult?> GetOneUntrackedAsync<TResult>(
            Expression<Func<T, TResult>> selector,
            RepositoryQuery<T> query,
            CancellationToken ct = default);

        Task<T?> GetOneUntrackedAsync(
            RepositoryQuery<T> query,
            CancellationToken ct = default);

        Task<(IEnumerable<T> items, int totalCount)> GetPagedAsync(
            RepositoryQuery<T>? query = null,
            int pageNumber = 1,
            int pageSize = 12,
            CancellationToken ct = default);

        Task<(IEnumerable<TResult> items, int totalCount)> GetPagedAsync<TResult>(
            Expression<Func<T, TResult>> selector,
            RepositoryQuery<T>? query = null,
            int pageNumber = 1,
            int pageSize = 12,
            CancellationToken ct = default);

        Task<int> GetCountAsync(
            RepositoryQuery<T>? query = null,
            CancellationToken ct = default);

        Task<bool> IsExistsAsync<TValue>(
            string key,
            TValue value,
            CancellationToken ct = default);

        Task<bool> IsExistsForUpdateAsync<TId, TValue>(
            TId id,
            string key,
            TValue value,
            CancellationToken ct = default);


        Task<T> CreateAsync(T model, CancellationToken ct = default);
        Task<IEnumerable<T>> CreateRangeAsync(IEnumerable<T> models, CancellationToken ct = default);
        Task<T> UpdateAsync(T model, CancellationToken ct = default);
        Task<IEnumerable<T>> UpdateRangeAsync(IEnumerable<T> models, CancellationToken ct = default);
        Task DeleteAsync(T model, CancellationToken ct = default);
        Task DeleteRangeAsync(IEnumerable<T> models, CancellationToken ct = default);
        Task SaveChangeAsync(CancellationToken ct = default);

        void AddEntity(T entity);
        void AddRangeEntity(IEnumerable<T> entities);
        void UpdateEntity(T entity);
        void UpdateRangeEntity(IEnumerable<T> entities);
        void DeleteEntity(T entity);
        void DeleteRangeEntity(IEnumerable<T> entities);
    }
}