using MediatR;
using Project.Application.Common.DTOs.Products;
using Project.Application.Features.Products.Shared.Interfaces;

namespace Project.Application.Features.Products.Commands.UpdateProduct
{
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ProductDto>
    {
        private readonly IProductWriteService _productWriteService;

        public UpdateProductCommandHandler(IProductWriteService productWriteService)
        {
            _productWriteService = productWriteService;
        }

        public async Task<ProductDto> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            return await _productWriteService.UpdateProductAsync(request.Id, request.Request, cancellationToken);
        }
    }
}
