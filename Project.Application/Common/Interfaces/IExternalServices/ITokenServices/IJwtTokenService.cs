using Project.Domain.Entities.Identity_Auth;

namespace Project.Application.Common.Interfaces.IExternalServices.ITokenServices
{
    public interface IJwtTokenService
    {
        string GenerateJwtToken(User user, IList<string> roles);
    }
}
