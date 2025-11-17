using Project.Domain.Entities.System_Log;
using Project.Domain.Interfaces.IRepositories.IBaseRepositories;

namespace Project.Domain.Interfaces.IRepositories.ISystem_LogRepositories
{
    public interface IAuditLogRepository : IBaseRepository<AuditLog>
    {
    }
}
