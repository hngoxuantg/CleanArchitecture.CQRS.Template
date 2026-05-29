using MediatR;
using Project.Application.Features.Products.Shared.Interfaces;

namespace Project.Application.Features.Products.Commands
{
    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, bool>
    {
        private readonly IProductWriteService _productWriteService;

        public DeleteProductCommandHandler(IProductWriteService productWriteService)
        {
            _productWriteService = productWriteService;
        }

        public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            return await _productWriteService.DeleteProductAsync(request.Id, cancellationToken);
        }
    }
}
