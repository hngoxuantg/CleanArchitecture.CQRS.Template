using Project.Application.Common.DTOs.Categories;
using Project.Application.Features.Categories.Request;

namespace Project.Application.Features.Categories.Shared.Interfaces
{
    public interface ICategoryCreationService
    {
        Task<CategoryDto> CreateCategoryAsync(CreateCategoryRequest request, CancellationToken cancellationToken = default);
    }
}
