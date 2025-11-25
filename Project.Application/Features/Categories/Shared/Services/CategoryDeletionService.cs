using Project.Application.Common.Exceptions;
using Project.Application.Features.Categories.Shared.Interfaces;
using Project.Domain.Entities.Business;
using Project.Domain.Interfaces.IRepositories.IBaseRepositories;

namespace Project.Application.Features.Categories.Shared.Services
{
    public class CategoryDeletionService : ICategoryDeletionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryDeletionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> DeleteCategoryAsync(int id, CancellationToken cancellationToken = default)
        {
            Category category = await _unitOfWork.CategoryRepository.GetByIdAsync(id, cancellationToken)
                ?? throw new NotFoundException($"Category with ID {id} not found.");

            await _unitOfWork.CategoryRepository.DeleteAsync(category, cancellationToken);

            return true;
        }
    }
}
