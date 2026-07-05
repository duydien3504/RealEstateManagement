using RealEstateSystem.Application.DTOs.Mail;
using RealEstateSystem.Application.DTOs.Request;
using RealEstateSystem.Application.DTOs.Response;
using RealEstateSystem.Application.Interfaces;
using RealEstateSystem.Domain.Entity;
using RealEstateSystem.Domain.Enums;

namespace RealEstateSystem.Application.Services.AuthenService
{
    public class RegisterService
    {
        private readonly IUserRepository _userRepository;
        private readonly IHasherPassword _passwordHasher;
        private readonly IRedisCacheService _redisCacheService;
        private readonly IMessagePublisher _messagePublisher;

        private const string OtpEmailQueueName = "otp-email-queue";
        private const int OtpExpiryMinutes = 5;

        public RegisterService(
            IUserRepository userRepository,
            IHasherPassword passwordHasher,
            IRedisCacheService redisCacheService,
            IMessagePublisher messagePublisher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _redisCacheService = redisCacheService;
            _messagePublisher = messagePublisher;
        }

        public async Task<RegisterResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken)
        {
            var emailExists = await _userRepository.CheckExistEmailAsync(request.Email, cancellationToken);
            if (emailExists)
            {
                throw new ArgumentException("Email này đã được sử dụng trong hệ thống.");
            }

            var roleId = await _userRepository.GetRoleIdByRoleTypeAsync(RoleType.User, cancellationToken);
            if (roleId == null)
            {
                throw new InvalidOperationException("Vai trò mặc định của người dùng (User) chưa được cấu hình trong hệ thống.");
            }

            var passwordHash = _passwordHasher.Hash(request.Password);
            var userId = Guid.NewGuid();

            var user = new User
            {
                UserId = userId,
                RoleId = roleId.Value,
                FullName = request.FullName,
                Email = request.Email,
                PasswordHash = passwordHash,
                Status = StatusType.Inactive,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var rawOtpCode = new Random().Next(100000, 999999).ToString();
            var hashedOtpCode = _passwordHasher.Hash(rawOtpCode);

            var otpVerification = new OtpVerification
            {
                OtpId = Guid.NewGuid(),
                UserId = userId,
                OtpCode = hashedOtpCode,
                Purpose = OtpPurpose.Register,
                ExpiredAt = DateTime.UtcNow.AddMinutes(OtpExpiryMinutes),
                IsUsed = false,
                CreatedAt = DateTime.UtcNow,
                User = user
            };

            user.OtpVerifications.Add(otpVerification);

            await _userRepository.AddAsync(user, cancellationToken);
            await _userRepository.SaveChangesAsync(cancellationToken);

            var redisKey = $"otp:register:{request.Email}";
            await _redisCacheService.SetAsync(redisKey, rawOtpCode, TimeSpan.FromMinutes(OtpExpiryMinutes), cancellationToken);

            var emailMessage = new SendOtpEmailMessage
            {
                ToEmail = request.Email,
                FullName = request.FullName,
                OtpCode = rawOtpCode
            };

            await _messagePublisher.PublishAsync(OtpEmailQueueName, emailMessage, cancellationToken);

            return new RegisterResponse
            {
                UserId = user.UserId,
                Email = user.Email,
                FullName = user.FullName,
                Message = "Đăng ký tài khoản thành công. Vui lòng xác thực mã OTP gửi về email của bạn."
            };
        }
    }
}
