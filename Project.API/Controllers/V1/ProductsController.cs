using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Project.Application.Common.DTOs.Products;
using Project.Application.Common.Interfaces.IExternalServices.ICacheServices;
using Project.Application.Features.Products.Commands;
using Project.Application.Features.Products.Queries.GetById;
using Project.Application.Features.Products.Request;
using Project.Common.Constants;
using Project.Common.Models.Responses;

namespace Project.API.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ProductsController : ControllerBase
    {
        private readonly ISender _sender;
        private readonly ICacheService _cache;

        public ProductsController(ISender sender, ICacheService cache)
        {
            _sender = sender;
            _cache = cache;
        }

        [EnableRateLimiting(RateLimitPolicies.PerUser)]
        [Authorize(Roles = Roles.Admin)]
        [HttpPost]
        public async Task<IActionResult> CreateProductAsync([
            FromBody] CreateProductRequest request,
            CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(new CreateProductCommand(request), cancellationToken);

            return Ok(new ApiResponse<ProductDto>
            {
                Success = true,
                Message = "Product created successfully",
                Data = result
            });
        }

        [EnableRateLimiting(RateLimitPolicies.PerUser)]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetProductByIdAsync([FromRoute] int id)
        {
            ProductDto? result;

            if (_cache.Exists($"Product_{id}"))
            {
                result = _cache.Get<ProductDto>($"Product_{id}");
            }
            else
            {
                result = await _sender.Send(new GetProductByIdQuery(id));

                if (result != null)
                    _cache.Set($"Product_{id}", result, TimeSpan.FromMinutes(10));
            }

            return Ok(new ApiResponse<ProductDto>
            {
                Success = true,
                Message = "Product retrieved successfully",
                Data = result
            });
        }

        [EnableRateLimiting(RateLimitPolicies.PerUser)]
        [Authorize(Roles = Roles.Admin)]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateProductAsync([
            FromRoute] int id,
            [FromBody] UpdateProductRequest request,
            CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(new UpdateProductCommand(id, request), cancellationToken);

            _cache.Remove($"Product_{id}");

            return Ok(new ApiResponse<ProductDto>
            {
                Success = true,
                Message = "Product updated successfully",
                Data = result
            });
        }

        [EnableRateLimiting(RateLimitPolicies.PerUser)]
        [Authorize(Roles = Roles.Admin)]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteProductAsync(
            [FromRoute] int id,
            CancellationToken cancellationToken = default)
        {
            await _sender.Send(new DeleteProductCommand(id), cancellationToken);

            _cache.Remove($"Product_{id}");

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Product deleted successfully"
            });
        }

        [EnableRateLimiting(RateLimitPolicies.PerUser)]
        [Authorize(Roles = Roles.Admin)]
        [HttpPost("{id:int}/images")]
        public async Task<IActionResult> UploadProductImagesAsync(
            [FromRoute] int id,
            [FromForm] IList<IFormFile> files,
            CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(new UploadProductImagesCommand(id, files), cancellationToken);

            _cache.Remove($"Product_{id}");

            return Ok(new ApiResponse<IEnumerable<string>>
            {
                Success = true,
                Message = "Product images uploaded successfully",
                Data = result
            });
        }

        [EnableRateLimiting(RateLimitPolicies.PerUser)]
        [HttpGet("{id:int}/images")]
        public async Task<IActionResult> GetProductImagesAsync([FromRoute] int id)
        {
            var product = await _sender.Send(new GetProductByIdQuery(id));

            return Ok(new ApiResponse<IEnumerable<string>>
            {
                Success = true,
                Message = "Product images retrieved successfully",
                Data = product?.ImageUrls ?? Enumerable.Empty<string>()
            });
        }
    }
}
