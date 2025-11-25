using Project.Application.Common.DTOs.Categories;
using Project.Application.Common.Exceptions;
using Project.Application.Features.Categories.Shared.Interfaces;
using Project.Domain.Interfaces.IRepositories.IBaseRepositories;

namespace Project.Application.Features.Categories.Shared.Services
{
    public class CategoryQueryService : ICategoryQueryService
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryQueryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<CategoryDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _unitOfWork.CategoryRepository.GetOneUntrackedAsync(
                filter: c => c.Id == id && !c.IsDeleted,
                selector: c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description
                },
                cancellation: cancellationToken) ?? throw new NotFoundException($"Category with id {id} not found.");

        }
    }
}
