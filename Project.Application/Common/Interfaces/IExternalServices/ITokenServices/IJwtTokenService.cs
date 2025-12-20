using Project.Domain.Entities.Identity_Auth;

namespace Project.Application.Common.Interfaces.IExternalServices.ITokenServices
{
    public interface IJwtTokenService
    {
        Task<string> GenerateJwtTokenAsync(User user, CancellationToken cancellation = default);
    }
}
