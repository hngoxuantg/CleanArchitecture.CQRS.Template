using AutoMapper;
using Project.Application.Common.DTOs.Categories;
using Project.Application.Common.Exceptions;
using Project.Application.Features.Categories.Shared.Interfaces;
using Project.Domain.Interfaces.IRepositories.IBaseRepositories;

namespace Project.Application.Features.Categories.Shared.Services
{
    public class CategoryReadService : ICategoryReadService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoryReadService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
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
