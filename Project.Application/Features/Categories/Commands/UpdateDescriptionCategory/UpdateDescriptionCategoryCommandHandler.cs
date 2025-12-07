using MediatR;
using Project.Application.Common.DTOs.Categories;
using Project.Application.Features.Categories.Shared.Interfaces;

namespace Project.Application.Features.Categories.Commands.UpdateDescriptionCategory
{
    public class UpdateDescriptionCategoryCommandHandler : IRequestHandler<UpdateDescriptionCategoryCommand, CategoryDto>
    {
        private readonly ICategoryWriteService _categoryWriteService;
        public UpdateDescriptionCategoryCommandHandler(ICategoryWriteService categoryWriteService)
        {
            _categoryWriteService = categoryWriteService;
        }
        public async Task<CategoryDto> Handle(UpdateDescriptionCategoryCommand request, CancellationToken cancellationToken)
        {
            return await _categoryWriteService.UpdateDescriptionCategoryAsync(
                request.Id,
                request.Request.Description,
                cancellationToken);
        }
    }
}
