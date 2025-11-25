using Microsoft.EntityFrameworkCore.Storage;
using Project.Domain.Interfaces.IRepositories.IBaseRepositories;
using Project.Domain.Interfaces.IRepositories.IBusinessRepositories;
using Project.Domain.Interfaces.IRepositories.IIdentity_AuthRepositories;
using Project.Domain.Interfaces.IRepositories.ISystem_LogRepositories;
using Project.Infrastructure.Data.Contexts;

namespace Project.Infrastructure.Data.Repositories.BaseRepositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;
        private IDbContextTransaction? _transaction;
        public UnitOfWork(IRoleRepository roleRepository,
            IUserRepository userRepository,
            ICategoryRepository categoryRepository,
            IProductRepository productRepository,
            IRefreshTokenRepository refreshTokenRepository,
            IAuditLogRepository auditLogRepository,
            ApplicationDbContext dbContext)
        {
            RoleRepository = roleRepository;
            UserRepository = userRepository;
            CategoryRepository = categoryRepository;
            ProductRepository = productRepository;
            RefreshTokenRepository = refreshTokenRepository;
            AuditLogRepository = auditLogRepository;
            _dbContext = dbContext;
        }
        public IRoleRepository RoleRepository { get; }
        public IUserRepository UserRepository { get; }
        public ICategoryRepository CategoryRepository { get; }
        public IProductRepository ProductRepository { get; }
        public IRefreshTokenRepository RefreshTokenRepository { get; }
        public IAuditLogRepository AuditLogRepository { get; }
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.SaveChangesAsync(cancellationToken);
        }
        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            _transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        }
        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync(cancellationToken);
                _transaction.Dispose();
                _transaction = null;
            }
        }
        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync(cancellationToken);
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _dbContext?.Dispose();
        }
    }
}
