using Project.Domain.Entities.System_Log;
using Project.Domain.Interfaces.IRepositories.ISystem_LogRepositories;
using Project.Infrastructure.Data.Contexts;
using Project.Infrastructure.Data.Repositories.BaseRepositories;

namespace Project.Infrastructure.Data.Repositories.System_LogRepositories
{
    public class AuditLogRepository : BaseRepository<AuditLog>, IAuditLogRepository
    {
        public AuditLogRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
