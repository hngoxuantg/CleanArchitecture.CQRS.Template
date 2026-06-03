using MediatR;
using Project.Application.Common.DTOs.Categories;
using Project.Application.Features.Categories.Requests;

namespace Project.Application.Features.Categories.Commands.UpdateCategory
{
    public record UpdateCategoryCommand(int Id, UpdateCategoryRequest Request) : IRequest<CategoryDto>;
}
