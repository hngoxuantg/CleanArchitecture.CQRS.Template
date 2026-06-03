using MediatR;

namespace Project.Application.Features.Products.Commands.DeleteProduct
{
    public record DeleteProductCommand(int Id) : IRequest<bool>;
}
