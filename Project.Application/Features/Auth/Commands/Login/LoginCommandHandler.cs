using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Project.Application.Common.DTOs.Auth;
using Project.Application.Common.Exceptions;
using Project.Application.Common.Interfaces.IExternalServices.ITokenServices;
using Project.Application.Common.Interfaces.IServices;
using Project.Common.Options;
using Project.Domain.Entities.Identity_Auth;
using Project.Domain.Interfaces.IRepositories.IBaseRepositories;

namespace Project.Application.Features.Auth.Commands.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly ICurrentUserService _currentUserService;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly AppSettings _appSettings;
        public LoginCommandHandler(
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

        public async Task<AuthDto> Handle(
            LoginCommand command,
            CancellationToken cancellationToken)
        {
            return await LoginAsync(
                command.Request,
                _currentUserService.DeviceInfo,
                _currentUserService.IpAddress,
                cancellationToken);
        }

        private async Task<AuthDto> LoginAsync(
            LoginRequest request,
            string? deviceInfo,
            string? ipAddress,
            CancellationToken cancellationToken = default)
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                if (_currentUserService.IsAuthenticated)
                    throw new ValidatorException("User is already authenticated");

                User? user = await _unitOfWork.UserRepository.GetOneAsync<User>(
                    filter: u => u.UserName == request.UserName,
                    cancellation: cancellationToken)
                    ?? throw new NotFoundException($"User with UserName {request.UserName} not found");

                if (!await _userManager.CheckPasswordAsync(user, request.Password))
                    throw new ValidatorException(nameof(LoginRequest.Password), $"Invalid password for user {request.UserName}");

                if (!await _userManager.IsEmailConfirmedAsync(user))
                    throw new BusinessRuleException("Email not verified!");

                string accessToken = await _jwtTokenService.GenerateJwtTokenAsync(user, cancellationToken);
                string refreshToken = _jwtTokenService.GenerateRefreshToken();

                RefreshToken newToken = new RefreshToken(
                    user.Id, refreshToken,
                    DateTime.UtcNow.AddDays(_appSettings.JwtConfig.RefreshTokenExpirationDays),
                    deviceInfo,
                    ipAddress);

                _unitOfWork.RefreshTokenRepository.AddEntity(newToken);

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                return new AuthDto
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
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
