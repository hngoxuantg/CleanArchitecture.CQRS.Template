using MediatR;
using Project.Application.Features.Categories.Shared.Interfaces;

namespace Project.Application.Features.Categories.Commands
{
    public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, bool>
    {
        private readonly ICategoryWriteService _categoryWriteService;

        public DeleteCategoryCommandHandler(ICategoryWriteService categoryWriteService)
        {
            _categoryWriteService = categoryWriteService;
        }

        public async Task<bool> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            return await _categoryWriteService.DeleteCategoryAsync(request.Id, cancellationToken);
        }
    }
}
