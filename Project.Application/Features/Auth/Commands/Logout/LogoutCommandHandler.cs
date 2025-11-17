using MediatR;
using Project.Application.Common.Exceptions;
using Project.Domain.Entities.Identity_Auth;
using Project.Domain.Interfaces.IRepositories.IBaseRepositories;

namespace Project.Application.Features.Auth.Commands.Logout
{
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        public LogoutCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(LogoutCommand command, CancellationToken cancellationToken)
        {
            return await LogoutAsync(command.RefreshToken, cancellationToken);
        }

        private async Task<bool> LogoutAsync(string? refreshToken, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(refreshToken))
                throw new ValidatorException("Refresh token is required");

            RefreshToken? oldToken = await _unitOfWork.RefreshTokenRepository.GetOneAsync<RefreshToken>(
                filter: r => r.Token == refreshToken,
                cancellation: cancellationToken) ?? throw new NotFoundException("Refresh token not found");

            if (!oldToken.IsActive)
                throw new ValidatorException("Refresh token is not active or has expired");

            oldToken.Revoke();

            await _unitOfWork.RefreshTokenRepository.UpdateAsync(oldToken, cancellationToken);

            return true;
        }
    }
}
