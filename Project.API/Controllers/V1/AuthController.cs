using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using Project.Application.Features.Auth.Commands.Login;
using Project.Application.Features.Auth.Commands.Logout;
using Project.Application.Features.Auth.Commands.Refresh;
using Project.Application.Features.Auth.Requests;
using Project.Common.Constants;
using Project.Common.Models.Responses;
using Project.Common.Options;

namespace Project.API.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AuthController : ControllerBase
    {
        private readonly AppSettings _appSettings;
        private readonly ISender _sender;
        public AuthController(IOptions<AppSettings> appSettings, ISender sender)
        {
            _appSettings = appSettings.Value;
            _sender = sender;
        }

        [EnableRateLimiting(RateLimitPolicies.Login)]
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync(
            [FromBody] LoginRequest loginRequest,
            CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(new LoginCommand(loginRequest), cancellationToken);

            Response.Cookies.Append("refreshToken", result.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddDays(_appSettings.JwtConfig.RefreshTokenExpirationDays)
            });

            return Ok(new AuthResult
            {
                Success = true,
                Message = "Login successful",
                AccessToken = result.AccessToken
            });
        }
 
        [EnableRateLimiting(RateLimitPolicies.PerUser)]
        [HttpPost("logout")]
        public async Task<IActionResult> LogoutAsync(CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(Request.Cookies["refreshToken"]))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Refresh token is required for logout"
                });
            }

            await _sender.Send(new LogoutCommand(Request.Cookies["refreshToken"]), cancellationToken);

            Response.Cookies.Delete("refreshToken");

            return Ok(new ApiResponse
            {
                Message = "Logout successful",
                Success = true
            });
        }

        [EnableRateLimiting(RateLimitPolicies.PerIp)]
        [AllowAnonymous]
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshAsync(CancellationToken cancellationToken = default)
        {
            string? refreshToken = Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(refreshToken))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Refresh token is required"
                });
            }

            var result = await _sender.Send(new RefreshCommand(refreshToken), cancellationToken);

            Response.Cookies.Append("refreshToken", result.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            });

            return Ok(new AuthResult
            {
                Success = true,
                Message = "Token refreshed successfully",
                AccessToken = result.AccessToken,
            });
        }
    }
}
