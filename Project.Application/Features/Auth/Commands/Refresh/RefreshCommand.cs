using MediatR;
using Project.Application.Common.DTOs.Auth;

namespace Project.Application.Features.Auth.Commands.Refresh
{
    public record RefreshCommand(string RefreshToken) : IRequest<AuthDto>;
}
