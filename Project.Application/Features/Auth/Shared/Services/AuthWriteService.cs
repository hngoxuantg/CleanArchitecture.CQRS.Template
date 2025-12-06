using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Project.Application.Common.DTOs.Auth;
using Project.Application.Common.Exceptions;
using Project.Application.Common.Interfaces.IExternalServices.ITokenServices;
using Project.Application.Common.Interfaces.IServices;
using Project.Application.Features.Auth.Requests;
using Project.Application.Features.Auth.Shared.Interfaces;
using Project.Common.Options;
using Project.Domain.Entities.Identity_Auth;
using Project.Domain.Interfaces.IRepositories.IBaseRepositories;

namespace Project.Application.Features.Auth.Shared.Services
{
    public class AuthWriteService : IAuthWriteService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly ICurrentUserService _currentUserService;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly AppSettings _appSettings;

        public AuthWriteService(
            IUnitOfWork unitOfWork,
            UserManager<User> userManager,
            ICurrentUserService currentUserService,
            IJwtTokenService jwtTokenService,
            IOptions<AppSettings> appSettings)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _currentUserService = currentUserService;
            _jwtTokenService = jwtTokenService;
            _appSettings = appSettings.Value;
        }

        public async Task<AuthDto> LoginAsync(
            LoginRequest request,
            string? deviceInfo,
            string? ipAddress,
            CancellationToken cancellationToken = default)
        {
            User user = await GetUserAsync(request.UserName, cancellationToken);

            await CheckLockoutAsync(user);

            await CheckPasswordAsync(user, request.Password);

            await CheckEmailConfirmedAsync(user);

            AuthDto authDto = await CreateTokensAsync(user, deviceInfo, ipAddress, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return authDto;
        }

        public async Task<bool> LogoutAsync(string? refreshToken, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(refreshToken))
                throw new ValidatorException("Refresh token is required");

            RefreshToken? oldToken = await GetRefreshTokenAsync(refreshToken, cancellationToken);

            if (!oldToken.IsActive)
                throw new ValidatorException("Refresh token is not active or has expired");

            oldToken.Revoke();

            await _unitOfWork.RefreshTokenRepository.UpdateAsync(oldToken, cancellationToken);

            return true;
        }

        public async Task<AuthDto> RefreshAsync(
            string? refreshToken,
            string? deviceInfo,
            string? ipAddress,
            CancellationToken cancellationToken = default)
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                RefreshToken? oldToken = await GetRefreshTokenAsync(refreshToken, cancellationToken);

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

        private async Task<User> GetUserAsync(string userName, CancellationToken cancellationToken)
        {
            if (_currentUserService.IsAuthenticated)
                throw new ValidatorException("User is already authenticated");

            User user = await _userManager.FindByNameAsync(userName)
                ?? throw new NotFoundException($"User with UserName {userName} not found");

            return user;
        }

        private async Task CheckLockoutAsync(User user)
        {
            if (await _userManager.IsLockedOutAsync(user))
                throw new BusinessRuleException("User account is locked out!");
        }

        private async Task CheckPasswordAsync(User user, string password)
        {
            if (!await _userManager.CheckPasswordAsync(user, password))
            {
                await _userManager.AccessFailedAsync(user);
                throw new ValidatorException(nameof(LoginRequest.Password), $"Invalid password for user {user.UserName}");
            }
            else
            {
                await _userManager.ResetAccessFailedCountAsync(user);
            }
        }
        private async Task CheckEmailConfirmedAsync(User user)
        {
            if (!await _userManager.IsEmailConfirmedAsync(user))
                throw new BusinessRuleException("Email not verified!");
        }
        private async Task<AuthDto> CreateTokensAsync(
            User user, string? deviceInfo, string? ipAddress, CancellationToken cancellationToken)
        {
            string accessToken = await _jwtTokenService.GenerateJwtTokenAsync(user, cancellationToken);
            string refreshToken = _jwtTokenService.GenerateRefreshToken();

            RefreshToken newToken = new RefreshToken(
                user.Id,
                refreshToken,
                DateTime.UtcNow.AddDays(_appSettings.JwtConfig.RefreshTokenExpirationDays),
                deviceInfo,
                ipAddress);

            _unitOfWork.RefreshTokenRepository.AddEntity(newToken);

            return new AuthDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        private async Task<RefreshToken> GetRefreshTokenAsync(string? refreshToken, CancellationToken cancellationToken = default)
        {
            return await _unitOfWork.RefreshTokenRepository.GetOneAsync<RefreshToken>(
                filter: r => r.Token == refreshToken,
                cancellation: cancellationToken) ?? throw new NotFoundException("Refresh token not found");
        }
    }
}
