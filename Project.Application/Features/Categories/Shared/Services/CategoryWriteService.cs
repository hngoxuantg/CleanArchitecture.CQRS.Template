using AutoMapper;
using Project.Application.Common.DTOs.Categories;
using Project.Application.Common.Exceptions;
using Project.Application.Features.Categories.Request;
using Project.Application.Features.Categories.Shared.Interfaces;
using Project.Domain.Entities.Business;
using Project.Domain.Interfaces.IRepositories.IBaseRepositories;

namespace Project.Application.Features.Categories.Shared.Services
{
    public class CategoryWriteService : ICategoryWriteService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoryWriteService(
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<CategoryDto> CreateCategoryAsync(
            CreateCategoryRequest request,
            CancellationToken cancellationToken = default)
        {
            if (await IsValidCategoryNameAsync(request.Name))
                throw new ValidatorException(
                    nameof(CreateCategoryRequest.Name), $"Category with name {request.Name} already exists.");

            Category category = _mapper.Map<Category>(request);

            await _unitOfWork.CategoryRepository.CreateAsync(category, cancellationToken);

            return _mapper.Map<CategoryDto>(category);
        }

        public async Task<CategoryDto> UpdateCategoryAsync(
            int id,
            UpdateCategoryRequest request,
            CancellationToken cancellationToken = default)
        {
            if (await IsValidCategoryNameAsync(id, request.Name, cancellationToken))
                throw new ValidatorException(
                    nameof(UpdateCategoryRequest.Name), $"Category with name {request.Name} already exists.");

            Category category = await GetCategoryByIdAsync(id, cancellationToken);

            _mapper.Map(request, category);

            await _unitOfWork.CategoryRepository.UpdateAsync(category, cancellationToken);

            return _mapper.Map<CategoryDto>(category);
        }

        public async Task<CategoryDto> UpdateDescriptionCategoryAsync(
            int id,
            string description,
            CancellationToken cancellationToken = default)
        {
            Category category = await GetCategoryByIdAsync(id, cancellationToken);

            category.SetDescription(description);

            await _unitOfWork.CategoryRepository.UpdateAsync(category, cancellationToken);

            return _mapper.Map<CategoryDto>(category);
        }

        public async Task<bool> DeleteCategoryAsync(int id, CancellationToken cancellationToken = default)
        {
            Category category = await GetCategoryByIdAsync(id, cancellationToken);

            await _unitOfWork.CategoryRepository.DeleteAsync(category, cancellationToken);

            return true;
        }

        private async Task<Category> GetCategoryByIdAsync(int id, CancellationToken cancellation = default)
        {
            return await _unitOfWork.CategoryRepository.GetByIdAsync(id, cancellation)
                ?? throw new NotFoundException($"Category with ID {id} not found.");
        }

        private async Task<bool> IsValidCategoryNameAsync(string name)
        {
            return await _unitOfWork.CategoryRepository.IsExistsAsync(nameof(Category.Name), name);
        }

        private async Task<bool> IsValidCategoryNameAsync(int id, string name, CancellationToken cancellation = default)
        {
            return await _unitOfWork.CategoryRepository.IsExistsForUpdateAsync(
                id,
                nameof(Category.Name),
                name,
                cancellation);
        }
    }
}
