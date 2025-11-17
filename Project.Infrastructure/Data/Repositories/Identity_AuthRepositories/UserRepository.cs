using Project.Domain.Entities.Identity_Auth;
using Project.Domain.Interfaces.IRepositories.IIdentity_AuthRepositories;
using Project.Infrastructure.Data.Contexts;
using Project.Infrastructure.Data.Repositories.BaseRepositories;

namespace Project.Infrastructure.Data.Repositories.Identity_AuthRepositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
