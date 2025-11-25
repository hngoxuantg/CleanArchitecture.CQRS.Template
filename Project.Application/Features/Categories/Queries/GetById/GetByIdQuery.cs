using MediatR;
using Project.Application.Common.DTOs.Categories;

namespace Project.Application.Features.Categories.Queries.GetById
{
    public record GetCategoryByIdQuery(int Id) : IRequest<CategoryDto>;
}
