using MediatR;
using Project.Application.Common.DTOs.Products;
using Project.Application.Features.Products.Requests;

namespace Project.Application.Features.Products.Commands.UpdateProduct
{
    public record UpdateProductCommand(int Id, UpdateProductRequest Request) : IRequest<ProductDto>;
}
