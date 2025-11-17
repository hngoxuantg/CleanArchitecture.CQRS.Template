using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Project.Application.DTOs;
using Project.Application.Interfaces.IServices;
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
        private readonly IAuthService _authService;
        private readonly AppSettings _appSettings;
        public AuthController(IAuthService authService, IOptions<AppSettings> appSettings)
        {
            _authService = authService;
            _appSettings = appSettings.Value;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync(
            [FromBody] LoginDto loginDto,
            CancellationToken cancellationToken = default)
        {
            string deviceInfo = Request.Headers["User-Agent"].ToString();
            string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown IP";

            (string accessToken, string refreshToken) = await _authService.LoginAsync(
                loginDto,
                deviceInfo,
                ipAddress,
                cancellationToken);

            Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
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
                AccessToken = accessToken,
                RefreshToken = refreshToken,
            });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> LogoutAsync(CancellationToken cancellationToken = default)
        {
            await _authService.LogoutAsync(Request.Cookies["refreshToken"], cancellationToken);

            Response.Cookies.Delete("refreshToken");

            return Ok(new ApiResponse
            {
                Message = "Logout successful",
                Success = true
            });
        }

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

            string deviceInfo = Request.Headers["User-Agent"].ToString();
            string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown IP";

            (string accessToken, string newRefreshToken) = await _authService.RefreshAsync(
                refreshToken,
                deviceInfo,
                ipAddress,
                cancellationToken);

            Response.Cookies.Append("refreshToken", newRefreshToken, new CookieOptions
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
                AccessToken = accessToken,
                RefreshToken = newRefreshToken
            });
        }
    }
}
