using Project.Application.Common.DTOs.Categories;
using Project.Application.Features.Categories.Request;

namespace Project.Application.Features.Categories.Shared.Interfaces
{
    public interface ICategoryUpdateService
    {
        Task<CategoryDto> UpdateCategoryAsync(int id, UpdateCategoryRequest request, CancellationToken cancellationToken = default);
    }
}
