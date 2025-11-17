using Project.Application.DTOs;

namespace Project.Application.Interfaces.IServices
{
    public interface IAuthService
    {
        Task<(string, string)> LoginAsync(
            LoginDto loginDto,
            string deviceInfo,
            string ipAddress,
            CancellationToken cancellationToken = default);

        Task<bool> LogoutAsync(
            string? refreshToken,
            CancellationToken cancellationToken = default);

        Task<(string, string)> RefreshAsync(
            string? refreshToken,
            string? deviceInfo,
            string? ipAddress,
            CancellationToken cancellationToken = default);
    }
}
