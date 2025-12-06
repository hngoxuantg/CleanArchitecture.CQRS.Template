using MediatR;
using Project.Application.Common.DTOs.Auth;
using Project.Application.Common.Interfaces.IServices;
using Project.Application.Features.Auth.Shared.Interfaces;

namespace Project.Application.Features.Auth.Commands.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthDto>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuthWriteService _authWriteService;

        public LoginCommandHandler(ICurrentUserService currentUserService, IAuthWriteService authWriteService)
        {
            _currentUserService = currentUserService;
            _authWriteService = authWriteService;
        }

        public async Task<AuthDto> Handle(
            LoginCommand request,
            CancellationToken cancellationToken)
        {
            return await _authWriteService.LoginAsync(
                request.Request,
                _currentUserService.DeviceInfo,
                _currentUserService.IpAddress,
                cancellationToken);
        }
    }
}
