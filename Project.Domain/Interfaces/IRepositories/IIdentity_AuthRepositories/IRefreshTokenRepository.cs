using Project.Domain.Entities.Identity_Auth;
using Project.Domain.Interfaces.IRepositories.IBaseRepositories;

namespace Project.Domain.Interfaces.IRepositories.IIdentity_AuthRepositories
{
    public interface IRefreshTokenRepository : IBaseRepository<RefreshToken>
    {
    }
}
