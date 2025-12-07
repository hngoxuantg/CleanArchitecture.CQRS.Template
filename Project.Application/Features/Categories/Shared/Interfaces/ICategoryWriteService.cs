using Project.Application.Common.DTOs.Categories;
using Project.Application.Features.Categories.Request;

namespace Project.Application.Features.Categories.Shared.Interfaces
{
    public interface ICategoryWriteService
    {
        Task<CategoryDto> CreateCategoryAsync(
            CreateCategoryRequest request,
            CancellationToken cancellationToken = default);

        Task<CategoryDto> UpdateCategoryAsync(
            int id,
            UpdateCategoryRequest request,
            CancellationToken cancellationToken = default);

        Task<bool> DeleteCategoryAsync(int id, CancellationToken cancellationToken = default);

        Task<CategoryDto> UpdateDescriptionCategoryAsync(
            int id,
            string description,
            CancellationToken cancellationToken = default);
    }
}
