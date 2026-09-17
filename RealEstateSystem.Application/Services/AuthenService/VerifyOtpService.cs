using Microsoft.Extensions.Logging;
using RealEstateSystem.Application.DTOs.Request;
using RealEstateSystem.Application.DTOs.Response;
using RealEstateSystem.Application.Interfaces;
using RealEstateSystem.Domain.Entity;
using RealEstateSystem.Domain.Enums;
using RealEstateSystem.Domain.Exceptions;

namespace RealEstateSystem.Application.Services.AuthenService
{
    public class VerifyOtpService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRedisCacheService _redisCacheService;
        private readonly ILogger<VerifyOtpService> _logger;

        public VerifyOtpService(
            IUserRepository userRepository, 
            IRedisCacheService redisCacheService,
            ILogger<VerifyOtpService> logger)
        {
            _userRepository = userRepository;
            _redisCacheService = redisCacheService;
            _logger = logger;
        }

        public async Task<VerifyOtpResponse> VerifyOtpAsync(VerifyOtpRequest request, string otp, CancellationToken cancellationToken)
        {
            var otpVerification = await _userRepository.GetLatestOtpVerificationWithUserAsync(request.Email, cancellationToken);

            if (otpVerification == null)
            {
                _logger.LogWarning("Xác thực OTP thất bại: Không tìm thấy thông tin OTP cho email {Email}.", request.Email);
                throw new NotFoundException("Không tìm thấy thông tin xác thực mã OTP cho tài khoản này.");
            }

            if (otpVerification.IsUsed)
            {
                _logger.LogWarning("Xác thực OTP thất bại: Mã OTP đã được sử dụng trước đó cho email {Email}.", request.Email);
                throw new BadRequestException("Mã OTP này đã được sử dụng.");
            }

            if (otpVerification.ExpiredAt < DateTime.UtcNow)
            {
                _logger.LogWarning("Xác thực OTP thất bại: Mã OTP trong DB đã hết hạn cho email {Email}.", request.Email);
                throw new BadRequestException("Mã OTP này đã hết hạn.");
            }

            var redisKey = $"otp:register:{request.Email}";
            var cachedOtp = await _redisCacheService.GetAsync(redisKey, cancellationToken);

            if (cachedOtp == null || cachedOtp != otp)
            {
                _logger.LogWarning("Xác thực OTP thất bại: Mã OTP không khớp cho email {Email}.", request.Email);
                throw new BadRequestException("Mã OTP không chính xác hoặc đã hết hạn.");
            }

            var user = otpVerification.User;
            if (user == null)
            {
                _logger.LogWarning("Xác thực OTP thất bại: Không tìm thấy thực thể người dùng liên kết với email {Email}.", request.Email);
                throw new NotFoundException("Tài khoản không tồn tại trên hệ thống.");
            }

            otpVerification.IsUsed = true;
            user.Status = StatusType.Active;

            if (user.Wallet == null)
            {
                user.Wallet = new Wallet
                {
                    UserId = user.UserId
                };
            }

            await _userRepository.SaveChangesAsync(cancellationToken);
            await _redisCacheService.DeleteAsync(redisKey, cancellationToken);

            _logger.LogInformation("Người dùng {Email} (ID: {UserId}) đã xác thực OTP thành công. Tài khoản đã kích hoạt.", user.Email, user.UserId);

            return new VerifyOtpResponse
            {
                Message = "Xác thực tài khoản thành công. Tài khoản của bạn đã được kích hoạt."
            };
        }
    }
}
