using Project.Application.Common.DTOs.Categories;

namespace Project.Application.Features.Categories.Shared.Interfaces
{
    public interface ICategoryQueryService
    {
        Task<CategoryDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    }
}
