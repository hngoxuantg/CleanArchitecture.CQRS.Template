using MediatR;
using Project.Application.Common.DTOs.Categories;
using Project.Application.Features.Categories.Shared.Interfaces;

namespace Project.Application.Features.Categories.Queries.GetById
{
    public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, CategoryDto>
    {
        private readonly ICategoryQueryService _categoryQueryService;
        public GetCategoryByIdQueryHandler(ICategoryQueryService categoryQueryService)
        {
            _categoryQueryService = categoryQueryService;
        }

        public async Task<CategoryDto> Handle(GetCategoryByIdQuery query, CancellationToken cancellationToken)
        {
            return await _categoryQueryService.GetByIdAsync(query.Id, cancellationToken);
        }
    }
}
