using RealEstateSystem.Application.DTOs.Mail;
using RealEstateSystem.Application.DTOs.Request;
using RealEstateSystem.Application.DTOs.Response;
using RealEstateSystem.Application.Interfaces;
using RealEstateSystem.Domain.Entity;
using RealEstateSystem.Domain.Enums;

namespace RealEstateSystem.Application.Services.AuthenService
{
    public class ForgetPasswordService
    {
        private readonly IUserRepository _userRepository;
        private readonly IHasherPassword _passwordHasher;
        private readonly IRedisCacheService _redisCacheService;
        private readonly IMessagePublisher _messagePublisher;

        private const string OtpEmailQueueName = "otp-email-queue";
        private const int OtpExpiryMinutes = 5;

        public ForgetPasswordService(
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

        public async Task<ForgetPasswordResponse> ForgetPasswordAsync(ForgetPasswordRequest request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserByEmailWithRoleAsync(request.Email, cancellationToken);
            if (user == null || user.IsDeleted || user.Status == StatusType.Deleted)
            {
                throw new ArgumentException("Email không tồn tại trong hệ thống.");
            }

            var rawOtpCode = new Random().Next(100000, 999999).ToString();
            var hashedOtpCode = _passwordHasher.Hash(rawOtpCode);

            var otpVerification = new OtpVerification
            {
                OtpId = Guid.NewGuid(),
                UserId = user.UserId,
                OtpCode = hashedOtpCode,
                Purpose = OtpPurpose.ForgotPassword,
                ExpiredAt = DateTime.UtcNow.AddMinutes(OtpExpiryMinutes),
                IsUsed = false,
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.AddOtpVerificationAsync(otpVerification, cancellationToken);
            await _userRepository.SaveChangesAsync(cancellationToken);

            var redisKey = $"otp:forget-password:{request.Email}";
            await _redisCacheService.SetAsync(redisKey, rawOtpCode, TimeSpan.FromMinutes(OtpExpiryMinutes), cancellationToken);

            var emailMessage = new SendOtpEmailMessage
            {
                ToEmail = request.Email,
                FullName = user.FullName,
                OtpCode = rawOtpCode
            };

            await _messagePublisher.PublishAsync(OtpEmailQueueName, emailMessage, cancellationToken);

            return new ForgetPasswordResponse
            {
                Message = "Mã OTP đã được gửi đến email của bạn để xác thực lại mật khẩu."
            };
        }
    }
}
