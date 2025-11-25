using AutoMapper;
using Project.Application.Common.DTOs.Categories;
using Project.Application.Common.Exceptions;
using Project.Application.Features.Categories.Request;
using Project.Application.Features.Categories.Shared.Interfaces;
using Project.Domain.Entities.Business;
using Project.Domain.Interfaces.IRepositories.IBaseRepositories;

namespace Project.Application.Features.Categories.Shared.Services
{
    public class CategoryUpdateService : ICategoryUpdateService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoryUpdateService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<CategoryDto> UpdateCategoryAsync(int id, UpdateCategoryRequest request, CancellationToken cancellationToken = default)
        {
            if (await IsValidCategoryNameAsync(id, request.Name, cancellationToken))
                throw new ValidatorException(nameof(UpdateCategoryRequest.Name), $"Category with name {request.Name} already exists.");

            Category category = await _unitOfWork.CategoryRepository.GetByIdAsync(id, cancellationToken)
                ?? throw new NotFoundException($"Category with ID {id} not found.");

            _mapper.Map(request, category);

            await _unitOfWork.CategoryRepository.UpdateAsync(category, cancellationToken);

            return _mapper.Map<CategoryDto>(category);
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
