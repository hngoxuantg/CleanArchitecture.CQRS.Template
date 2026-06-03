using MediatR;
using Project.Application.Common.DTOs.Categories;
using Project.Application.Features.Categories.Requests;

namespace Project.Application.Features.Categories.Commands.CreateCategory
{
    public record CreateCategoryCommand(CreateCategoryRequest Request) : IRequest<CategoryDto>;
}
