using MediatR;
using Project.Application.Common.DTOs.Categories;
using Project.Application.Features.Categories.Shared.Interfaces;

namespace Project.Application.Features.Categories.Commands
{
    public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, CategoryDto>
    {
        private readonly ICategoryUpdateService _categoryUpdateService;

        public UpdateCategoryCommandHandler(ICategoryUpdateService categoryUpdateService)
        {
            _categoryUpdateService = categoryUpdateService;
        }

        public async Task<CategoryDto> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            return await _categoryUpdateService.UpdateCategoryAsync(request.Id, request.Request, cancellationToken);
        }
    }
}
