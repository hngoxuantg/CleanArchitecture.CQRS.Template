using MediatR;
using Project.Application.Common.DTOs.Categories;
using Project.Application.Features.Categories.Shared.Interfaces;

namespace Project.Application.Features.Categories.Queries.GetById
{
    public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, CategoryDto>
    {
        private readonly ICategoryReadService _categoryReadService;

        public GetCategoryByIdQueryHandler(ICategoryReadService categoryReadService)
        {
            _categoryReadService = categoryReadService;
        }

        public async Task<CategoryDto> Handle(GetCategoryByIdQuery query, CancellationToken cancellationToken)
        {
            return await _categoryReadService.GetByIdAsync(query.Id, cancellationToken);
        }
    }
}
