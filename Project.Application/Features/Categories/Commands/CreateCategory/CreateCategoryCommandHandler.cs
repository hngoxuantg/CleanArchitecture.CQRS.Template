using MediatR;
using Project.Application.Common.DTOs.Categories;
using Project.Application.Features.Categories.Shared.Interfaces;

namespace Project.Application.Features.Categories.Commands
{
    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, CategoryDto>
    {
        private readonly ICategoryCreationService _categoryCreationService;
        public CreateCategoryCommandHandler(ICategoryCreationService categoryCreationService)
        {
            _categoryCreationService = categoryCreationService;
        }

        public async Task<CategoryDto> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            return await _categoryCreationService.CreateCategoryAsync(request.Request, cancellationToken);
        }
    }
}
