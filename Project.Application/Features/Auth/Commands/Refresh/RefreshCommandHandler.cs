using MediatR;
using Microsoft.Extensions.Options;
using Project.Application.Common.DTOs.Auth;
using Project.Application.Common.Exceptions;
using Project.Application.Common.Interfaces.IExternalServices.ITokenServices;
using Project.Application.Common.Interfaces.IServices;
using Project.Common.Options;
using Project.Domain.Entities.Identity_Auth;
using Project.Domain.Interfaces.IRepositories.IBaseRepositories;

namespace Project.Application.Features.Auth.Commands.Refresh
{
    public class RefreshCommandHandler : IRequestHandler<RefreshCommand, AuthDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly AppSettings _appSettings;
        private readonly ICurrentUserService _currentUserService;
        public RefreshCommandHandler(
            IUnitOfWork unitOfWork,
            IJwtTokenService jwtTokenService,
            IOptions<AppSettings> appSettings,
            ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _jwtTokenService = jwtTokenService;
            _appSettings = appSettings.Value;
            _currentUserService = currentUserService;
        }

        public async Task<AuthDto> Handle(RefreshCommand command, CancellationToken cancellationToken)
        {
            return await RefreshAsync(
                command.RefreshToken,
                _currentUserService.DeviceInfo,
                _currentUserService.IpAddress,
                cancellationToken);
        }
        private async Task<AuthDto> RefreshAsync(
            string? refreshToken,
            string? deviceInfo,
            string? ipAddress,
            CancellationToken cancellationToken = default)
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                RefreshToken? oldToken = await _unitOfWork.RefreshTokenRepository.GetOneAsync<RefreshToken>(
                    filter: r => r.Token == refreshToken,
                    cancellation: cancellationToken) ?? throw new NotFoundException("Refresh token not found");

                if (!oldToken.IsActive)
                    throw new ValidatorException("Refresh token is not active or has expired");

                User? user = await _unitOfWork.UserRepository.GetByIdAsync(
                    oldToken.UserId,
                    cancellation: cancellationToken)
                    ?? throw new NotFoundException("User not found");

                oldToken.Revoke();

                string accessToken = await _jwtTokenService.GenerateJwtTokenAsync(user, cancellationToken);
                string newRefreshToken = _jwtTokenService.GenerateRefreshToken();

                RefreshToken newRefreshTokenEntity = new RefreshToken(
                    user.Id, newRefreshToken,
                    DateTime.UtcNow.AddDays(_appSettings.JwtConfig.RefreshTokenExpirationDays),
                    deviceInfo,
                    ipAddress);

                _unitOfWork.RefreshTokenRepository.AddEntity(newRefreshTokenEntity);

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                return new AuthDto
                {
                    AccessToken = accessToken,
                    RefreshToken = newRefreshToken
                };
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }
    }
}
