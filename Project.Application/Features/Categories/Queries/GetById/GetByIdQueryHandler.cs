using AutoMapper;
using MediatR;
using Project.Application.Common.DTOs.Categories;
using Project.Application.Common.Exceptions;
using Project.Domain.Entities.Business;
using Project.Domain.Interfaces.IRepositories.IBaseRepositories;

namespace Project.Application.Features.Categories.Queries.GetById
{
    public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, CategoryDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public GetCategoryByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<CategoryDto> Handle(GetCategoryByIdQuery query, CancellationToken cancellationToken)
        {
            return await GetByIdAsync(query.Id, cancellationToken);
        }
        private async Task<CategoryDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            Category? category = await _unitOfWork.CategoryRepository.GetOneUntrackedAsync<Category>(
                filter: c => c.Id == id && !c.IsDeleted,
                cancellation: cancellationToken) ?? throw new NotFoundException($"Category with id {id} not found.");

            return _mapper.Map<CategoryDto>(category);
        }
    }
}
