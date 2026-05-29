using MediatR;

namespace Project.Application.Features.Products.Commands
{
    public record DeleteProductCommand(int Id) : IRequest<bool>;
}
