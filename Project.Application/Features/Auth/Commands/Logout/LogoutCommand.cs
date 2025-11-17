using MediatR;

namespace Project.Application.Features.Auth.Commands.Logout
{
    public record LogoutCommand(string RefreshToken) : IRequest<bool>;
}
