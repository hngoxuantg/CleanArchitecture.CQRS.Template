using Microsoft.EntityFrameworkCore;
using Project.Application.Common.DTOs.Products;
using Project.Application.Common.Exceptions;
using Project.Application.Common.Interfaces.IExternalServices.IStorageServices;
using Project.Application.Features.Products.Shared.Interfaces;
using Project.Domain.Entities.Business;
using Project.Domain.Interfaces.IRepositories.IBaseRepositories;

namespace Project.Application.Features.Products.Shared.Services
{
    public class ProductReadService : IProductReadService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileService _fileService;

        public ProductReadService(IUnitOfWork unitOfWork, IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _fileService = fileService;
        }

        public async Task<ProductDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            RepositoryQuery<Product> query = RepositoryQuery<Product>.For()
                .Where(p => p.Id == id && !p.IsDeleted);

            return await _unitOfWork.ProductRepository.GetOneUntrackedAsync(
                query: query,
                selector: p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    CategoryId = p.CategoryId,
                    ImageUrls = _fileService.GetAbsoluteUrls(p.ProductImages.Select(pi => pi.ImageUrl)).ToList()
                },
                ct: cancellationToken) ?? throw new NotFoundException($"Product with id {id} not found.");
        }
    }
}
