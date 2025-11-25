using AutoMapper;
using Project.Application.Common.DTOs.Categories;
using Project.Application.Common.Exceptions;
using Project.Application.Features.Categories.Request;
using Project.Application.Features.Categories.Shared.Interfaces;
using Project.Domain.Entities.Business;
using Project.Domain.Interfaces.IRepositories.IBaseRepositories;

namespace Project.Application.Features.Categories.Shared.Services
{
    public class CategoryCreationService : ICategoryCreationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoryCreationService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryRequest request, CancellationToken cancellationToken = default)
        {
            if (await IsValidCategoryNameAsync(request.Name))
                throw new ValidatorException(nameof(CreateCategoryRequest.Name), $"Category with name {request.Name} already exists.");

            Category category = _mapper.Map<Category>(request);

            await _unitOfWork.CategoryRepository.CreateAsync(category, cancellationToken);

            return _mapper.Map<CategoryDto>(category);
        }

        private async Task<bool> IsValidCategoryNameAsync(string name)
        {
            return await _unitOfWork.CategoryRepository.IsExistsAsync(nameof(Category.Name), name);
        }
    }
}
