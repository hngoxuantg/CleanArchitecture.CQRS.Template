using MediatR;
using Project.Application.Common.DTOs.Products;
using Project.Application.Features.Products.Request;

namespace Project.Application.Features.Products.Commands
{
    public record UpdateProductCommand(int Id, UpdateProductRequest Request) : IRequest<ProductDto>;
}
