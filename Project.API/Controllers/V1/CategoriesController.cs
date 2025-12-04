using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Caching.Memory;
using Project.Application.Common.DTOs.Categories;
using Project.Application.Features.Categories.Commands;
using Project.Application.Features.Categories.Queries.GetById;
using Project.Application.Features.Categories.Request;
using Project.Common.Constants;
using Project.Common.Models.Responses;

namespace Project.API.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CategoriesController : ControllerBase
    {
        private readonly ISender _sender;
        private readonly IMemoryCache _cache;

        public CategoriesController(ISender sender, IMemoryCache cache)
        {
            _sender = sender;
            _cache = cache;
        }

        [EnableRateLimiting(RateLimitPolicies.PerUser)]
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateCategoryAsync(
            [FromBody] CreateCategoryRequest createCategoryRequest,
            CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(new CreateCategoryCommand(createCategoryRequest), cancellationToken);

            return Ok(new ApiResponse<CategoryDto>
            {
                Success = true,
                Message = "Category created successfully",
                Data = result
            });
        }

        [EnableRateLimiting(RateLimitPolicies.PerUser)]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCategoryByIdAsync([FromRoute] int id)
        {
            var result = new CategoryDto();

            if (_cache.TryGetValue($"Category_{id}", out CategoryDto? category))
            {
                result = category;
            }
            else
            {
                result = await _sender.Send(new GetCategoryByIdQuery(id));

                if (result != null)
                    _cache.Set($"Category_{id}", result, TimeSpan.FromMinutes(10));
            }

            return Ok(new ApiResponse<CategoryDto>
            {
                Success = true,
                Message = "Category retrieved successfully",
                Data = result
            });
        }

        [EnableRateLimiting(RateLimitPolicies.PerUser)]
        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateCategoryAsync(
            [FromRoute] int id,
            [FromBody] UpdateCategoryRequest updateCategoryRequest,
            CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(new UpdateCategoryCommand(id, updateCategoryRequest), cancellationToken);

            _cache.Remove($"Category_{id}");

            return Ok(new ApiResponse<CategoryDto>
            {
                Success = true,
                Message = "Category updated successfully",
                Data = result
            });
        }

        [EnableRateLimiting(RateLimitPolicies.PerUser)]
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCategoryAsync(
            [FromRoute] int id,
            CancellationToken cancellationToken = default)
        {
            await _sender.Send(new DeleteCategoryCommand(id), cancellationToken);

            _cache.Remove($"Category_{id}");

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Category deleted successfully"
            });
        }
    }
}
