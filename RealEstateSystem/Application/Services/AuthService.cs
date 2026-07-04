using RealEstateSystem.Application.DTOs.Request;
using RealEstateSystem.Application.DTOs.Response;
using RealEstateSystem.Application.Interfaces;
using RealEstateSystem.Domain.Entities;
using RealEstateSystem.Infrastructure.Repositories;
using RealEstateSystem.Domain.Enums;

namespace RealEstateSystem.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenProvider _tokenProvider;

        public AuthService(UserRepository userRepository, IPasswordHasher passwordHasher, ITokenProvider tokenProvider)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _tokenProvider = tokenProvider;
        }

        public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
        {
            var isExist = await _userRepository.IsEmailExistAsync(request.Email);
            if(isExist)
            {
                return new RegisterResponse { IsSuccess = false, Message = "Email đã được đăng ký trên hệ thống.", Email = request.Email };
            }

            string hashedPassword = _passwordHasher.HasHPassword(request.Password);
            var user = new User(
                fullName: request.Fullname,
                email: request.Email,
                passwordHash: hashedPassword
                );

            await _userRepository.AddUserAsync(user);
            await _userRepository.SaveChangesAsync();

            return new RegisterResponse
            {
                IsSuccess = true,
                Message = "Đăng ký thành công. Vui lòng kiểm tra email để xác thực.",
                Email = request.Email
            };
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            var user = await _userRepository.GetUserByEmailAsync(request.email);
            if(user == null)
            {
                throw new Exception("Tài khoản không tồn tại");
            }

            if(user.Status == StatusAccount.Inactive)
            {
                throw new Exception("Tài khoản chưa được kích hoạt");
            }

            if(!_passwordHasher.VerifyPassword(request.password, user.PasswordHash))
            {
                throw new Exception("Sai mật khẩu");
            }

            var accessToken = _tokenProvider.GenerateAccessToken(user);
            var refreshTokenString = _tokenProvider.GenerateRefreshToken();

            var refreshTokenEntity = new RefreshToken(user.UserId, refreshTokenString, DateTime.UtcNow.AddDays(7));

            user.UpdateLastLogin();
            await _userRepository.UpdateUserAsync(user);

            return new LoginResponse
            {
                Success = true,
                Message = "Đăng nhập thành công!",
                AccessToken = accessToken,
                RefreshToken = refreshTokenString
            };
        }
    }
}
