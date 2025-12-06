using Project.Application.Common.DTOs.Auth;
using Project.Application.Features.Auth.Requests;

namespace Project.Application.Features.Auth.Shared.Interfaces
{
    public interface IAuthWriteService
    {
        Task<AuthDto> LoginAsync(
            LoginRequest request,
            string? deviceInfo,
            string? ipAddress,
            CancellationToken cancellationToken = default);

        Task<bool> LogoutAsync(string? refreshToken, CancellationToken cancellationToken = default);

        Task<AuthDto> RefreshAsync(
            string? refreshToken,
            string? deviceInfo,
            string? ipAddress,
            CancellationToken cancellationToken = default);
    }
}
