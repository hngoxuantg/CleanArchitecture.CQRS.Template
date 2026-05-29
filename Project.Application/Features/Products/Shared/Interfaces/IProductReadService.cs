using Project.Application.Common.DTOs.Products;

namespace Project.Application.Features.Products.Shared.Interfaces
{
    public interface IProductReadService
    {
        Task<ProductDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    }
}
