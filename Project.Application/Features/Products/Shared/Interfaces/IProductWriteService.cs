using Project.Application.Common.DTOs.Products;
using Project.Application.Features.Products.Requests;

namespace Project.Application.Features.Products.Shared.Interfaces
{
    public interface IProductWriteService
    {
        Task<ProductDto> CreateProductAsync(CreateProductRequest request, CancellationToken cancellationToken = default);

        Task<ProductDto> UpdateProductAsync(int id, UpdateProductRequest request, CancellationToken cancellationToken = default);

        Task<bool> DeleteProductAsync(int id, CancellationToken cancellationToken = default);

        Task<IEnumerable<string>> UploadImagesAsync(int productId, IList<Microsoft.AspNetCore.Http.IFormFile> formFiles, CancellationToken cancellationToken = default);
    }
}
