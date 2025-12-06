using MediatR;
using Project.Application.Features.Auth.Shared.Interfaces;

namespace Project.Application.Features.Auth.Commands.Logout
{
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, bool>
    {
        private readonly IAuthWriteService _authWriteService;
        public LogoutCommandHandler(IAuthWriteService authWriteService)
        {
            _authWriteService = authWriteService;
        }

        public async Task<bool> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            return await _authWriteService.LogoutAsync(request.RefreshToken, cancellationToken);
        }
    }
}
