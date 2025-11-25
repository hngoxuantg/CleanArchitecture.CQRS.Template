using MediatR;
using Project.Application.Common.DTOs.Auth;
using Project.Application.Common.Interfaces.IServices;
using Project.Application.Features.Auth.Shared.Interfaces;

namespace Project.Application.Features.Auth.Commands.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthDto>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuthService _authService;
        public LoginCommandHandler(ICurrentUserService currentUserService, IAuthService authService)
        {
            _currentUserService = currentUserService;
            _authService = authService;
        }

        public async Task<AuthDto> Handle(
            LoginCommand request,
            CancellationToken cancellationToken)
        {
            return await _authService.LoginAsync(
                request.Request,
                _currentUserService.DeviceInfo,
                _currentUserService.IpAddress,
                cancellationToken);
        }
    }
}
