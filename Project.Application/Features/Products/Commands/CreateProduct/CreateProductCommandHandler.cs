using MediatR;
using Project.Application.Common.DTOs.Products;
using Project.Application.Features.Products.Shared.Interfaces;

namespace Project.Application.Features.Products.Commands
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
    {
        private readonly IProductWriteService _productWriteService;

        public CreateProductCommandHandler(IProductWriteService productWriteService)
        {
            _productWriteService = productWriteService;
        }

        public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            return await _productWriteService.CreateProductAsync(request.Request, cancellationToken);
        }
    }
}
