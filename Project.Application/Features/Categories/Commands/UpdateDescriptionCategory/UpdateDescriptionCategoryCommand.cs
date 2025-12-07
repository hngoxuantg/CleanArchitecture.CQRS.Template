using MediatR;
using Project.Application.Common.DTOs.Categories;
using Project.Application.Features.Categories.Request;

namespace Project.Application.Features.Categories.Commands.UpdateDescriptionCategory
{
    public record UpdateDescriptionCategoryCommand(
        int Id,
        UpdateDescriptionCategoryRequest Request) : IRequest<CategoryDto>;
}
