using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.Application.Common.DTOs.Categories;
using Project.Application.Features.Categories.Commands;
using Project.Application.Features.Categories.Queries.GetById;
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
        public CategoriesController(ISender sender)
        {
            _sender = sender;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateCategoryAsync(
            [FromBody] CreateCategoryRequest createCategoryRequest,
            CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(new CreateCategoryCommand(createCategoryRequest), cancellationToken);

            return Ok(new ApiResponse<CategoryDto>
            {
                Message = "Category retrieved successfully",
                Success = true,
                Data = result,
            });
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCategoryByIdAsync([FromRoute] int id)
        {
            var result = await _sender.Send(new GetCategoryByIdQuery(id));

            return Ok(new ApiResponse<CategoryDto>
            {
                Message = "Category retrieved successfully",
                Success = true,
                Data = result,
            });
        }
    }
}
