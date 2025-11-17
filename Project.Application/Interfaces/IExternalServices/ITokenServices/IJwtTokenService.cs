using Project.Domain.Entities.Identity_Auth;

namespace Project.Application.Interfaces.IExternalServices.ITokenServices
{
    public interface IJwtTokenService
    {
        Task<string> GenerateJwtTokenAsync(User user, CancellationToken cancellation = default);
        string GenerateRefreshToken();
    }
}
