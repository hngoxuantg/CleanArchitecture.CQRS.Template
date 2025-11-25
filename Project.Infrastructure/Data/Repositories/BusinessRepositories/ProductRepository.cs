using Project.Domain.Entities.Business;
using Project.Domain.Interfaces.IRepositories.IBusinessRepositories;
using Project.Infrastructure.Data.Contexts;
using Project.Infrastructure.Data.Repositories.BaseRepositories;

namespace Project.Infrastructure.Data.Repositories.BusinessRepositories
{
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
