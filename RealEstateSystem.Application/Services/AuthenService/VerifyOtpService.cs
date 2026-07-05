using RealEstateSystem.Application.DTOs.Request;
using RealEstateSystem.Application.DTOs.Response;
using RealEstateSystem.Application.Interfaces;
using RealEstateSystem.Domain.Enums;

namespace RealEstateSystem.Application.Services.AuthenService
{
    public class VerifyOtpService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRedisCacheService _redisCacheService;

        public VerifyOtpService(IUserRepository userRepository, IRedisCacheService redisCacheService)
        {
            _userRepository = userRepository;
            _redisCacheService = redisCacheService;
        }

        public async Task<VerifyOtpResponse> VerifyOtpAsync(VerifyOtpRequest request, string otp, CancellationToken cancellationToken)
        {
            var otpVerification = await _userRepository.GetLatestOtpVerificationWithUserAsync(request.Email, cancellationToken);

            if (otpVerification == null)
            {
                throw new ArgumentException("Không tìm thấy thông tin xác thực mã OTP cho tài khoản này.");
            }

            if (otpVerification.IsUsed)
            {
                throw new ArgumentException("Mã OTP này đã được sử dụng.");
            }

            if (otpVerification.ExpiredAt < DateTime.UtcNow)
            {
                throw new ArgumentException("Mã OTP này đã hết hạn.");
            }

            var redisKey = $"otp:register:{request.Email}";
            var cachedOtp = await _redisCacheService.GetAsync(redisKey, cancellationToken);

            if (cachedOtp == null || cachedOtp != otp)
            {
                throw new ArgumentException("Mã OTP không chính xác hoặc đã hết hạn.");
            }

            var user = otpVerification.User;
            if (user == null)
            {
                throw new ArgumentException("Tài khoản không tồn tại trên hệ thống.");
            }

            otpVerification.IsUsed = true;
            user.Status = StatusType.Active;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateUserAsync(user, cancellationToken);
            await _userRepository.SaveChangesAsync(cancellationToken);
            await _redisCacheService.DeleteAsync(redisKey, cancellationToken);

            return new VerifyOtpResponse
            {
                Message = "Xác thực tài khoản thành công. Tài khoản của bạn đã được kích hoạt."
            };
        }
    }
}
