using MediatR;
using Project.Application.Common.DTOs.Auth;
using Project.Application.Features.Auth.Requests;

namespace Project.Application.Features.Auth.Commands.Login
{
    public record LoginCommand(LoginRequest Request) : IRequest<AuthDto>;
}
