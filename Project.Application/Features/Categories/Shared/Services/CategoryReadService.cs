using AutoMapper;
using Project.Application.Common.DTOs.Categories;
using Project.Application.Common.Exceptions;
using Project.Application.Features.Categories.Shared.Interfaces;
using Project.Domain.Entities.Business;
using Project.Domain.Interfaces.IRepositories.IBaseRepositories;

namespace Project.Application.Features.Categories.Shared.Services
{
    public class CategoryReadService : ICategoryReadService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryReadService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CategoryDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            RepositoryQuery<Category> query = RepositoryQuery<Category>.For()
                .Where(c => c.Id == id && !c.IsDeleted);

            return await _unitOfWork.CategoryRepository.GetOneUntrackedAsync(
                query: query,
                selector: c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description
                },
                ct: cancellationToken) ?? throw new NotFoundException($"Category with id {id} not found.");

        }
    }
}
