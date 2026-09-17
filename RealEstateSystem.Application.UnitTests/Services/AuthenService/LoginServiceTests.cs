using Microsoft.Extensions.Logging;
using RealEstateSystem.Application.DTOs.Request;
using RealEstateSystem.Application.Interfaces;
using RealEstateSystem.Application.Services.AuthenService;
using RealEstateSystem.Application.UnitTests.Common;
using RealEstateSystem.Domain.Entity;
using RealEstateSystem.Domain.Enums;
using RealEstateSystem.Domain.Exceptions;

namespace RealEstateSystem.Application.UnitTests.Services.AuthenService
{
    public class LoginServiceTests
    {
        private readonly IUserRepository _userRepository;
        private readonly IHasherPassword _passwordHasher;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly ILogger<LoginService> _logger;
        private readonly LoginService _loginService;

        public LoginServiceTests()
        {
            _userRepository = Substitute.For<IUserRepository>();
            _passwordHasher = Substitute.For<IHasherPassword>();
            _jwtTokenGenerator = Substitute.For<IJwtTokenGenerator>();
            _logger = Substitute.For<ILogger<LoginService>>();
            _loginService = new LoginService(_userRepository, _passwordHasher, _jwtTokenGenerator, _logger);
        }

        [Fact]
        public async Task LoginAsync_ValidRequest_ReturnsLoginResponse()
        {
            var userId = Guid.NewGuid();
            var email = "test@example.com";
            var password = "Password123!";
            var hashedPassword = "hashed_password";
            var user = TestDataBuilder.CreateActiveUser(userId, email, "Test User", hashedPassword);

            var request = new LoginRequest { Email = email, Password = password };
            var ipAddress = "127.0.0.1";

            _userRepository.GetUserByEmailWithRoleAsync(email, Arg.Any<CancellationToken>()).Returns(user);
            _passwordHasher.Verify(password, hashedPassword).Returns(true);
            _jwtTokenGenerator.GenerateAccessToken(user).Returns("access_token");
            _jwtTokenGenerator.GenerateRefreshToken().Returns("refresh_token");

            var response = await _loginService.LoginAsync(request, ipAddress, CancellationToken.None);

            response.Should().NotBeNull();
            response.AccessToken.Should().Be("access_token");
            response.RefreshToken.Should().Be("refresh_token");
            response.UserId.Should().Be(userId);
            response.Email.Should().Be(email);
            response.Message.Should().Be("Đăng nhập thành công.");

            await _userRepository.Received(1).AddRefreshTokenAsync(Arg.Any<RefreshToken>(), Arg.Any<CancellationToken>());
            await _userRepository.Received(1).UpdateUserAsync(user, Arg.Any<CancellationToken>());
            await _userRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task LoginAsync_UserNotFound_ThrowsUnauthorizedException()
        {
            var email = "notfound@example.com";
            var request = new LoginRequest { Email = email, Password = "AnyPassword" };
            
            _userRepository.GetUserByEmailWithRoleAsync(email, Arg.Any<CancellationToken>()).Returns((User?)null);

            var action = () => _loginService.LoginAsync(request, "127.0.0.1", CancellationToken.None);

            await action.Should().ThrowAsync<UnauthorizedException>()
                .WithMessage("Email hoặc mật khẩu không chính xác.");
        }

        [Fact]
        public async Task LoginAsync_UserInactive_ThrowsBadRequestException()
        {
            var userId = Guid.NewGuid();
            var email = "inactive@example.com";
            var user = TestDataBuilder.CreateInactiveUser(userId, email);
            var request = new LoginRequest { Email = email, Password = "AnyPassword" };

            _userRepository.GetUserByEmailWithRoleAsync(email, Arg.Any<CancellationToken>()).Returns(user);

            var action = () => _loginService.LoginAsync(request, "127.0.0.1", CancellationToken.None);

            await action.Should().ThrowAsync<BadRequestException>()
                .WithMessage("Tài khoản của bạn chưa được kích hoạt bằng OTP.");
        }

        [Fact]
        public async Task LoginAsync_UserBlocked_ThrowsBadRequestException()
        {
            var userId = Guid.NewGuid();
            var email = "blocked@example.com";
            var user = TestDataBuilder.CreateBlockedUser(userId, email);
            var request = new LoginRequest { Email = email, Password = "AnyPassword" };

            _userRepository.GetUserByEmailWithRoleAsync(email, Arg.Any<CancellationToken>()).Returns(user);

            var action = () => _loginService.LoginAsync(request, "127.0.0.1", CancellationToken.None);

            await action.Should().ThrowAsync<BadRequestException>()
                .WithMessage("Tài khoản của bạn đã bị khóa bởi quản trị viên.");
        }

        [Fact]
        public async Task LoginAsync_InvalidPassword_ThrowsUnauthorizedException()
        {
            var userId = Guid.NewGuid();
            var email = "test@example.com";
            var password = "WrongPassword";
            var hashedPassword = "hashed_password";
            var user = TestDataBuilder.CreateActiveUser(userId, email, "Test User", hashedPassword);

            var request = new LoginRequest { Email = email, Password = password };

            _userRepository.GetUserByEmailWithRoleAsync(email, Arg.Any<CancellationToken>()).Returns(user);
            _passwordHasher.Verify(password, hashedPassword).Returns(false);

            var action = () => _loginService.LoginAsync(request, "127.0.0.1", CancellationToken.None);

            await action.Should().ThrowAsync<UnauthorizedException>()
                .WithMessage("Email hoặc mật khẩu không chính xác.");
        }
    }
}
