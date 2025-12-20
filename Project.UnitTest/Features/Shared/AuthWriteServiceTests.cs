using AutoFixture;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Project.Application.Common.DTOs.Auth;
using Project.Application.Common.Exceptions;
using Project.Application.Common.Interfaces.IExternalServices.ITokenServices;
using Project.Application.Common.Interfaces.IServices;
using Project.Application.Features.Auth.Requests;
using Project.Application.Features.Auth.Shared.Services;
using Project.Common.Options;
using Project.Domain.Entities.Identity_Auth;
using Project.Domain.Interfaces.IRepositories.IBaseRepositories;
using System.Linq.Expressions;

namespace Project.UnitTest.Features.Auth
{
    public class AuthWriteServiceTests
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly ICurrentUserService _currentUserService;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IOptions<AppSettings> _appSettings;
        private readonly Fixture _fixture;
        private readonly AuthWriteService _authWriteService;

        public AuthWriteServiceTests()
        {
            _fixture = new Fixture();

            _unitOfWork = Substitute.For<IUnitOfWork>();

            IUserStore<User> userStore = Substitute.For<IUserStore<User>>();

            _userManager = Substitute.For<UserManager<User>>(
                userStore, null, null, null, null, null, null, null, null);

            _currentUserService = Substitute.For<ICurrentUserService>();
            _jwtTokenService = Substitute.For<IJwtTokenService>();

            _appSettings = Substitute.For<IOptions<AppSettings>>();
            _appSettings.Value.Returns(new AppSettings
            {
                JwtConfig = new JwtConfig { RefreshTokenExpirationDays = 7 }
            });

            _authWriteService = new AuthWriteService(
                _unitOfWork,
                _userManager,
                _currentUserService,
                _jwtTokenService,
                _appSettings);
        }

        [Fact]
        public async Task LoginAsync_ValidCredentials_ShouldReturnAuthDtoAndSaveToken()
        {
            LoginRequest request = _fixture.Create<LoginRequest>();
            User user = _fixture.Create<User>();
            string expectedAccessToken = _fixture.Create<string>();
            string deviceInfo = _fixture.Create<string>();
            string ipAddress = _fixture.Create<string>();

            _userManager.FindByNameAsync(request.UserName).Returns(user);
            _userManager.IsLockedOutAsync(user).Returns(false);
            _userManager.CheckPasswordAsync(user, request.Password).Returns(true);
            _userManager.IsEmailConfirmedAsync(user).Returns(true);
            _currentUserService.IsAuthenticated.Returns(false);

            _jwtTokenService.GenerateJwtTokenAsync(user, Arg.Any<CancellationToken>())
                .Returns(expectedAccessToken);

            AuthDto result = await _authWriteService.LoginAsync(request, deviceInfo, ipAddress);

            Assert.NotNull(result);
            Assert.Equal(expectedAccessToken, result.AccessToken);
            Assert.NotNull(result.RefreshToken);

            _unitOfWork.RefreshTokenRepository.Received(1).AddEntity(Arg.Is<RefreshToken>(rt =>
                rt.UserId == user.Id &&
                rt.DeviceInfo == deviceInfo
            ));

            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task LoginAsync_WrongPassword_ShouldThrowExceptionAndRecordFail()
        {
            LoginRequest request = _fixture.Create<LoginRequest>();
            User user = _fixture.Create<User>();
            string deviceInfo = _fixture.Create<string>();
            string ipAddress = _fixture.Create<string>();

            _userManager.FindByNameAsync(request.UserName).Returns(user);
            _userManager.IsLockedOutAsync(user).Returns(false);
            _currentUserService.IsAuthenticated.Returns(false);

            _userManager.CheckPasswordAsync(user, request.Password).Returns(false);

            ValidatorException exception = await Assert.ThrowsAsync<ValidatorException>(() =>
                _authWriteService.LoginAsync(request, deviceInfo, ipAddress));

            await _userManager.Received(1).AccessFailedAsync(user);
        }

        [Fact]
        public async Task LoginAsync_UserLockedOut_ShouldThrowBusinessRuleException()
        {
            LoginRequest request = _fixture.Create<LoginRequest>();
            User user = _fixture.Create<User>();
            string deviceInfo = _fixture.Create<string>();
            string ipAddress = _fixture.Create<string>();

            _userManager.FindByNameAsync(request.UserName).Returns(user);
            _userManager.IsLockedOutAsync(user).Returns(true);
            _currentUserService.IsAuthenticated.Returns(false);

            BusinessRuleException ex = await Assert.ThrowsAsync<BusinessRuleException>(() =>
                _authWriteService.LoginAsync(request, deviceInfo, ipAddress));
        }

        [Fact]
        public async Task LogoutAsync_ValidToken_ShouldRevokeToken()
        {
            string tokenString = _fixture.Create<string>();
            string deviceInfo = _fixture.Create<string>();
            string ipAddress = _fixture.Create<string>();
            int userId = _fixture.Create<int>();

            RefreshToken tokenEntity = new RefreshToken(userId, DateTime.UtcNow.AddDays(1), deviceInfo, ipAddress);

            _unitOfWork.RefreshTokenRepository
                .GetOneAsync<RefreshToken>(Arg.Any<Expression<Func<RefreshToken, bool>>>(), cancellation: Arg.Any<CancellationToken>())
                .Returns(tokenEntity);

            bool result = await _authWriteService.LogoutAsync(tokenString);

            Assert.True(result);
            Assert.NotNull(tokenEntity.Revoked);

            await _unitOfWork.RefreshTokenRepository.Received(1)
                .UpdateAsync(tokenEntity, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task LogoutAsync_TokenNotFound_ShouldThrowNotFoundException()
        {
            string tokenString = _fixture.Create<string>();

            _unitOfWork.RefreshTokenRepository
                .GetOneAsync<RefreshToken>(Arg.Any<Expression<Func<RefreshToken, bool>>>(), cancellation: Arg.Any<CancellationToken>())
                .Returns((RefreshToken)null);

            await Assert.ThrowsAsync<NotFoundException>(() => _authWriteService.LogoutAsync(tokenString));
        }

        [Fact]
        public async Task RefreshAsync_ValidToken_ShouldRotateTokenAndCommitTransaction()
        {
            string oldTokenString = _fixture.Create<string>();
            string newDevice = _fixture.Create<string>();
            string newIp = _fixture.Create<string>();
            string oldDevice = _fixture.Create<string>();
            string oldIp = _fixture.Create<string>();
            string newAccessToken = _fixture.Create<string>();
            int userId = _fixture.Create<int>();

            RefreshToken oldTokenEntity = new RefreshToken(userId, DateTime.UtcNow.AddDays(1), oldDevice, oldIp);
            User user = _fixture.Build<User>().With(u => u.Id, userId).Create();

            _unitOfWork.RefreshTokenRepository
                .GetOneAsync<RefreshToken>(Arg.Any<Expression<Func<RefreshToken, bool>>>(), cancellation: Arg.Any<CancellationToken>())
                .Returns(oldTokenEntity);

            _unitOfWork.UserRepository
                .GetByIdAsync(userId, cancellation: Arg.Any<CancellationToken>())
                .Returns(user);

            _jwtTokenService.GenerateJwtTokenAsync(user, Arg.Any<CancellationToken>())
                .Returns(newAccessToken);

            AuthDto result = await _authWriteService.RefreshAsync(oldTokenString, newDevice, newIp);

            Assert.NotNull(result);
            Assert.Equal(newAccessToken, result.AccessToken);
            Assert.NotEqual(oldTokenString, result.RefreshToken);

            Assert.NotNull(oldTokenEntity.Revoked);

            await _unitOfWork.Received(1).BeginTransactionAsync(Arg.Any<CancellationToken>());

            _unitOfWork.RefreshTokenRepository.Received(1).AddEntity(Arg.Is<RefreshToken>(rt =>
                rt.UserId == userId &&
                rt.Token != oldTokenString &&
                rt.DeviceInfo == newDevice
            ));

            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
            await _unitOfWork.Received(1).CommitTransactionAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task RefreshAsync_WhenExceptionOccurs_ShouldRollbackTransaction()
        {
            string tokenString = _fixture.Create<string>();
            string deviceInfo = _fixture.Create<string>();
            string ipAddress = _fixture.Create<string>();
            int userId = _fixture.Create<int>();

            RefreshToken tokenEntity = new RefreshToken(userId, DateTime.UtcNow.AddDays(1), deviceInfo, ipAddress);

            _unitOfWork.RefreshTokenRepository
                .GetOneAsync<RefreshToken>(Arg.Any<Expression<Func<RefreshToken, bool>>>(), cancellation: Arg.Any<CancellationToken>())
                .Returns(tokenEntity);

            _unitOfWork.UserRepository
                .GetByIdAsync(Arg.Any<int>(), cancellation: Arg.Any<CancellationToken>())
                .Throws(new Exception("Database error simulaton"));

            Exception ex = await Assert.ThrowsAsync<Exception>(() =>
                _authWriteService.RefreshAsync(tokenString, deviceInfo, ipAddress));

            Assert.Equal("Database error simulaton", ex.Message);

            await _unitOfWork.Received(1).RollbackTransactionAsync(Arg.Any<CancellationToken>());
            await _unitOfWork.DidNotReceive().CommitTransactionAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task RefreshAsync_TokenInactive_ShouldThrowValidatorException()
        {
            string tokenString = _fixture.Create<string>();
            string deviceInfo = _fixture.Create<string>();
            string ipAddress = _fixture.Create<string>();

            RefreshToken revokedToken = new RefreshToken(1, DateTime.UtcNow.AddDays(1), deviceInfo, ipAddress);
            revokedToken.Revoke();

            _unitOfWork.RefreshTokenRepository
                .GetOneAsync<RefreshToken>(Arg.Any<Expression<Func<RefreshToken, bool>>>(), cancellation: Arg.Any<CancellationToken>())
                .Returns(revokedToken);

            ValidatorException ex = await Assert.ThrowsAsync<ValidatorException>(() =>
                _authWriteService.RefreshAsync(tokenString, deviceInfo, ipAddress));

            Assert.Contains("not active", ex.Message);
        }
    }
}