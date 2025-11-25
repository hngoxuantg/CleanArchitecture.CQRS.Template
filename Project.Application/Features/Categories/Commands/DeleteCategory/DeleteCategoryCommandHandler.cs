using MediatR;
using Project.Application.Features.Categories.Shared.Interfaces;

namespace Project.Application.Features.Categories.Commands
{
    public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, bool>
    {
        private readonly ICategoryDeletionService _categoryDeletionService;

        public DeleteCategoryCommandHandler(ICategoryDeletionService categoryDeletionService)
        {
            _categoryDeletionService = categoryDeletionService;
        }

        public async Task<bool> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            return await _categoryDeletionService.DeleteCategoryAsync(request.Id, cancellationToken);
        }
    }
}
