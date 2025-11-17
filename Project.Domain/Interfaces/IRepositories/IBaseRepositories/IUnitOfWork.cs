using Project.Domain.Interfaces.IRepositories.IBusinessRepositories;
using Project.Domain.Interfaces.IRepositories.IIdentity_AuthRepositories;
using Project.Domain.Interfaces.IRepositories.ISystem_LogRepositories;

namespace Project.Domain.Interfaces.IRepositories.IBaseRepositories
{
    public interface IUnitOfWork
    {
        IRoleRepository RoleRepository { get; }
        IUserRepository UserRepository { get; }
        ICategoryRepository CategoryRepository { get; }
        IProductRepository ProductRepository { get; }
        IRefreshTokenRepository RefreshTokenRepository { get; }
        IAuditLogRepository AuditLogRepository { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);
        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
        void Dispose();
    }
}
