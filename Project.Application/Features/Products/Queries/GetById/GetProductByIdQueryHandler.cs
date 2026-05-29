using MediatR;
using Project.Application.Common.DTOs.Products;
using Project.Application.Features.Products.Shared.Interfaces;

namespace Project.Application.Features.Products.Queries.GetById
{
    public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto>
    {
        private readonly IProductReadService _productReadService;

        public GetProductByIdQueryHandler(IProductReadService productReadService)
        {
            _productReadService = productReadService;
        }

        public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            return await _productReadService.GetByIdAsync(request.Id, cancellationToken);
        }
    }
}
