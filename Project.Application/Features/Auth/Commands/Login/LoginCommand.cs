using MediatR;
using Project.Application.Common.DTOs.Auth;

namespace Project.Application.Features.Auth.Commands.Login
{
    public record LoginCommand(LoginRequest Request) : IRequest<AuthDto>;
}
