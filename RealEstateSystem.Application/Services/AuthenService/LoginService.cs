using RealEstateSystem.Application.DTOs.Request;
using RealEstateSystem.Application.DTOs.Response;
using RealEstateSystem.Application.Interfaces;
using RealEstateSystem.Domain.Entity;
using RealEstateSystem.Domain.Enums;

namespace RealEstateSystem.Application.Services.AuthenService
{
    public class LoginService
    {
        private readonly IUserRepository _userRepository;
        private readonly IHasherPassword _passwordHasher;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public LoginService(
            IUserRepository userRepository, 
            IHasherPassword passwordHasher, 
            IJwtTokenGenerator jwtTokenGenerator)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request, string ipAddress, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserByEmailWithRoleAsync(request.Email, cancellationToken);
            if (user == null || user.IsDeleted || user.Status == StatusType.Deleted)
            {
                throw new UnauthorizedAccessException("Email hoặc mật khẩu không chính xác.");
            }

            if (user.Status == StatusType.Inactive)
            {
                throw new ArgumentException("Tài khoản của bạn chưa được kích hoạt bằng OTP.");
            }

            if (user.Status == StatusType.Block)
            {
                throw new ArgumentException("Tài khoản của bạn đã bị khóa bởi quản trị viên.");
            }

            var isPasswordValid = _passwordHasher.Verify(request.Password, user.PasswordHash);
            if (!isPasswordValid)
            {
                throw new UnauthorizedAccessException("Email hoặc mật khẩu không chính xác.");
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
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.AddRefreshTokenAsync(refreshToken, cancellationToken);
            await _userRepository.UpdateUserAsync(user, cancellationToken);
            await _userRepository.SaveChangesAsync(cancellationToken);

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
