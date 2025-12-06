using MediatR;
using Project.Application.Common.DTOs.Categories;
using Project.Application.Features.Categories.Shared.Interfaces;

namespace Project.Application.Features.Categories.Commands
{
    public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, CategoryDto>
    {
        private readonly ICategoryWriteService _categoryWriteService;

        public UpdateCategoryCommandHandler(ICategoryWriteService categoryWriteService)
        {
            _categoryWriteService = categoryWriteService;
        }

        public async Task<CategoryDto> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            return await _categoryWriteService.UpdateCategoryAsync(request.Id, request.Request, cancellationToken);
        }
    }
}
