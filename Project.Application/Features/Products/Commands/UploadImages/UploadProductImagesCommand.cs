using MediatR;
using Microsoft.AspNetCore.Http;

namespace Project.Application.Features.Products.Commands.UploadImages
{
    public record UploadProductImagesCommand(int ProductId, IList<IFormFile> Files) : IRequest<IEnumerable<string>>;
}
