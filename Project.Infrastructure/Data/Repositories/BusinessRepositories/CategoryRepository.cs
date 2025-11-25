using Project.Domain.Entities.Business;
using Project.Domain.Interfaces.IRepositories.IBusinessRepositories;
using Project.Infrastructure.Data.Contexts;
using Project.Infrastructure.Data.Repositories.BaseRepositories;

namespace Project.Infrastructure.Data.Repositories.BusinessRepositories
{
    public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
