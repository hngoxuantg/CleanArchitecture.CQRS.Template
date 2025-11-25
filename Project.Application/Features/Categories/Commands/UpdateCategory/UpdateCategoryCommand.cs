using MediatR;
using Project.Application.Common.DTOs.Categories;
using Project.Application.Features.Categories.Request;

namespace Project.Application.Features.Categories.Commands
{
    public record UpdateCategoryCommand(int Id, UpdateCategoryRequest Request) : IRequest<CategoryDto>;
}
