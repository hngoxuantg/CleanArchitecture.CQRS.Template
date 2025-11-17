using MediatR;
using Project.Application.Common.DTOs.Categories;

namespace Project.Application.Features.Categories.Commands
{
    public record CreateCategoryCommand(CreateCategoryRequest Request) : IRequest<CategoryDto>;
}
