using MediatR;

namespace Project.Application.Features.Categories.Commands.DeleteCategory
{
    public record DeleteCategoryCommand(int Id) : IRequest<bool>;
}
