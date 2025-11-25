using MediatR;
using Project.Application.Common.DTOs.Auth;
using Project.Application.Common.Interfaces.IServices;
using Project.Application.Features.Auth.Shared.Interfaces;

namespace Project.Application.Features.Auth.Commands.Refresh
{
    public class RefreshCommandHandler : IRequestHandler<RefreshCommand, AuthDto>
    {
        private readonly IAuthService _authService;
        private readonly ICurrentUserService _currentUserService;
        public RefreshCommandHandler(ICurrentUserService currentUserService, IAuthService authService)
        {
            _authService = authService;
            _currentUserService = currentUserService;
        }

        public async Task<AuthDto> Handle(RefreshCommand request, CancellationToken cancellationToken)
        {
            return await _authService.RefreshAsync(
                request.RefreshToken,
                _currentUserService.DeviceInfo,
                _currentUserService.IpAddress,
                cancellationToken);
        }
    }
}
