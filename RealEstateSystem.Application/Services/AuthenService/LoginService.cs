using Microsoft.Extensions.Logging;
using RealEstateSystem.Application.DTOs.Request;
using RealEstateSystem.Application.DTOs.Response;
using RealEstateSystem.Application.Interfaces;
using RealEstateSystem.Domain.Entity;
using RealEstateSystem.Domain.Enums;
using RealEstateSystem.Domain.Exceptions;

namespace RealEstateSystem.Application.Services.AuthenService
{
    public class LoginService
    {
        private readonly IUserRepository _userRepository;
        private readonly IHasherPassword _passwordHasher;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly ILogger<LoginService> _logger;

        public LoginService(
            IUserRepository userRepository, 
            IHasherPassword passwordHasher, 
            IJwtTokenGenerator jwtTokenGenerator,
            ILogger<LoginService> logger)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtTokenGenerator = jwtTokenGenerator;
            _logger = logger;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request, string ipAddress, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserByEmailWithRoleAsync(request.Email, cancellationToken);
            if (user == null || user.IsDeleted || user.Status == StatusType.Deleted)
            {
                _logger.LogWarning("Đăng nhập thất bại: Email {Email} không tồn tại hoặc đã bị xóa.", request.Email);
                throw new UnauthorizedException("Email hoặc mật khẩu không chính xác.");
            }

            if (user.Status == StatusType.Inactive)
            {
                _logger.LogWarning("Đăng nhập thất bại: Tài khoản {Email} chưa được kích hoạt.", request.Email);
                throw new BadRequestException("Tài khoản của bạn chưa được kích hoạt bằng OTP.");
            }

            if (user.Status == StatusType.Block)
            {
                _logger.LogWarning("Đăng nhập thất bại: Tài khoản {Email} đã bị khóa.", request.Email);
                throw new BadRequestException("Tài khoản của bạn đã bị khóa bởi quản trị viên.");
            }

            var isPasswordValid = _passwordHasher.Verify(request.Password, user.PasswordHash);
            if (!isPasswordValid)
            {
                _logger.LogWarning("Đăng nhập thất bại: Sai mật khẩu cho tài khoản {Email}.", request.Email);
                throw new UnauthorizedException("Email hoặc mật khẩu không chính xác.");
            }

            var accessToken = _jwtTokenGenerator.GenerateAccessToken(user);
            var refreshTokenValue = _jwtTokenGenerator.GenerateRefreshToken();

            var refreshToken = new RefreshToken
            {
                RefreshTokenId = Guid.NewGuid(),
                UserId = user.UserId,
                Token = refreshTokenValue,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                IsRevoked = false,
                CreatedAt = DateTime.UtcNow,
                CreatedByIp = ipAddress
            };

            user.LastLogin = DateTime.UtcNow;

            await _userRepository.AddRefreshTokenAsync(refreshToken, cancellationToken);
            await _userRepository.UpdateUserAsync(user, cancellationToken);
            await _userRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Người dùng {Email} (ID: {UserId}) đã đăng nhập thành công từ IP {IpAddress}.", user.Email, user.UserId, ipAddress);

            return new LoginResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshTokenValue,
                UserId = user.UserId,
                Email = user.Email,
                FullName = user.FullName,
                RoleName = user.Role.NameRole.ToString(),
                Message = "Đăng nhập thành công."
            };
        }
    }
}
