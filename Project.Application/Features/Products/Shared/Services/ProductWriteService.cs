using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Project.Application.Common.DTOs.Products;
using Project.Application.Common.Exceptions;
using Project.Application.Common.Interfaces.IExternalServices.IStorageServices;
using Project.Application.Features.Products.Requests;
using Project.Application.Features.Products.Shared.Interfaces;
using Project.Domain.Entities.Business;
using Project.Domain.Interfaces.IRepositories.IBaseRepositories;

namespace Project.Application.Features.Products.Shared.Services
{
    public class ProductWriteService : IProductWriteService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IFileService _fileService;

        public ProductWriteService(IUnitOfWork unitOfWork, IMapper mapper, IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _fileService = fileService;
        }

        public async Task<ProductDto> CreateProductAsync(CreateProductRequest request, CancellationToken cancellationToken = default)
        {
            if (await _unitOfWork.ProductRepository.IsExistsAsync(nameof(Product.Name), request.Name, cancellationToken))
                throw new ValidatorException(nameof(CreateProductRequest.Name), $"Product with name {request.Name} already exists.");

            if (await _unitOfWork.CategoryRepository.IsExistsAsync(nameof(Category.Id), request.CategoryId, cancellationToken) == false)
                throw new ValidatorException(nameof(CreateProductRequest.CategoryId), $"Category with ID {request.CategoryId} does not exist.");

            Product product = _mapper.Map<Product>(request);

            await _unitOfWork.ProductRepository.CreateAsync(product, cancellationToken);

            return _mapper.Map<ProductDto>(product);
        }

        public async Task<ProductDto> UpdateProductAsync(int id, UpdateProductRequest request, CancellationToken cancellationToken = default)
        {
            if (await _unitOfWork.ProductRepository.IsExistsForUpdateAsync(id, nameof(Product.Name), request.Name, cancellationToken))
                throw new ValidatorException(nameof(UpdateProductRequest.Name), $"Product with name {request.Name} already exists.");

            if (await _unitOfWork.CategoryRepository.IsExistsAsync(nameof(Category.Id), request.CategoryId, cancellationToken) == false)
                throw new ValidatorException(nameof(UpdateProductRequest.CategoryId), $"Category with ID {request.CategoryId} does not exist.");

            Product product = await GetProductByIdAsync(id, cancellationToken);

            _mapper.Map(request, product);

            await _unitOfWork.ProductRepository.UpdateAsync(product, cancellationToken);

            ProductDto result = _mapper.Map<ProductDto>(product);

            result.ImageUrls = _fileService.GetAbsoluteUrls(product.ProductImages.Select(pi => pi.ImageUrl).ToList());

            return result;
        }

        public async Task<bool> DeleteProductAsync(int id, CancellationToken cancellationToken = default)
        {
            Product product = await GetProductByIdAsync(id, cancellationToken);

            await _unitOfWork.ProductRepository.DeleteAsync(product, cancellationToken);

            return true;
        }

        private async Task<Product> GetProductByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _unitOfWork.ProductRepository.GetOneUntrackedAsync<Product>(
                filter: p => p.Id == id && !p.IsDeleted,
                include: p => p.Include(p => p.ProductImages),
                cancellation: cancellationToken)
                ?? throw new NotFoundException($"Product with ID {id} not found.");
        }

        public async Task<IEnumerable<string>> UploadImagesAsync(int productId, IList<Microsoft.AspNetCore.Http.IFormFile> formFiles, CancellationToken cancellationToken = default)
        {
            if (formFiles == null || formFiles.Count == 0)
                return Enumerable.Empty<string>();

            Product product = await GetProductByIdAsync(productId, cancellationToken);

            var relativePaths = await _fileService.SaveImagesAsync(formFiles, $"products/{productId}", cancellationToken);

            product.AddImages(relativePaths.ToList());

            await _unitOfWork.ProductRepository.UpdateAsync(product, cancellationToken);

            return _fileService.GetAbsoluteUrls(relativePaths).ToList();
        }
    }
}
