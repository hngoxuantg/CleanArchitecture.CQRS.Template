using MediatR;
using Project.Application.Common.DTOs.Products;

namespace Project.Application.Features.Products.Queries.GetById
{
    public record GetProductByIdQuery(int Id) : IRequest<ProductDto>;
}
