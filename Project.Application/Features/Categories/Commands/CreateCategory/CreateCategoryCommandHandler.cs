using MediatR;
using Project.Application.Common.DTOs.Categories;
using Project.Application.Features.Categories.Shared.Interfaces;

namespace Project.Application.Features.Categories.Commands
{
    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, CategoryDto>
    {
        private readonly ICategoryWriteService _categoryWriteService;

        public CreateCategoryCommandHandler(ICategoryWriteService categoryWriteService)
        {
            _categoryWriteService = categoryWriteService;
        }

        public async Task<CategoryDto> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            return await _categoryWriteService.CreateCategoryAsync(request.Request, cancellationToken);
        }
    }
}
