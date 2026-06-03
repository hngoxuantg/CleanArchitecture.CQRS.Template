using MediatR;
using Project.Application.Features.Products.Shared.Interfaces;

namespace Project.Application.Features.Products.Commands.UploadImages
{
    public class UploadProductImagesCommandHandler : IRequestHandler<UploadProductImagesCommand, IEnumerable<string>>
    {
        private readonly IProductWriteService _productWriteService;

        public UploadProductImagesCommandHandler(IProductWriteService productWriteService)
        {
            _productWriteService = productWriteService;
        }

        public async Task<IEnumerable<string>> Handle(UploadProductImagesCommand request, CancellationToken cancellationToken)
        {
            return await _productWriteService.UploadImagesAsync(request.ProductId, request.Files, cancellationToken);
        }
    }
}
