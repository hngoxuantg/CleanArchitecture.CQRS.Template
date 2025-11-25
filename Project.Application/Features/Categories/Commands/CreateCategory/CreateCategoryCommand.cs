using MediatR;
using Project.Application.Common.DTOs.Categories;
using Project.Application.Features.Categories.Request;

namespace Project.Application.Features.Categories.Commands
{
    public record CreateCategoryCommand(CreateCategoryRequest Request) : IRequest<CategoryDto>;
}
