using MediatR;

namespace Project.Application.Features.Categories.Commands
{
    public record DeleteCategoryCommand(int Id) : IRequest<bool>;
}
