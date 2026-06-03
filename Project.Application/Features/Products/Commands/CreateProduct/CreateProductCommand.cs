using MediatR;
using Project.Application.Common.DTOs.Products;
using Project.Application.Features.Products.Requests;

namespace Project.Application.Features.Products.Commands.CreateProduct
{
    public record CreateProductCommand(CreateProductRequest Request) : IRequest<ProductDto>;
}
