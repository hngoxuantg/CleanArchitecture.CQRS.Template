namespace Project.Application.Features.Categories.Shared.Interfaces
{
    public interface ICategoryDeletionService
    {
        Task<bool> DeleteCategoryAsync(int id, CancellationToken cancellationToken = default);
    }
}
