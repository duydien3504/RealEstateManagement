using RealEstateSystem.Application.DTOs.Request;
using RealEstateSystem.Application.DTOs.Response;
using RealEstateSystem.Application.Interfaces;
using RealEstateSystem.Domain.Exceptions;

namespace RealEstateSystem.Application.Services.AuthenService
{
    public class VerifyChangePasswordService
    {
        private readonly IUserRepository _userRepository;
        private readonly IHasherPassword _passwordHasher;
        private readonly IRedisCacheService _redisCacheService;

        public VerifyChangePasswordService(
            IUserRepository userRepository,
            IHasherPassword passwordHasher,
            IRedisCacheService redisCacheService)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _redisCacheService = redisCacheService;
        }

        public async Task<VerifyChangePasswordResponse> VerifyChangePasswordAsync(
            VerifyChangePasswordRequest request,
            string otp,
            CancellationToken cancellationToken)
        {
            var redisKey = $"otp:forget-password:{request.Email}";
            var cachedOtp = await _redisCacheService.GetAsync(redisKey, cancellationToken);

            if (cachedOtp == null || cachedOtp != otp)
            {
                throw new BadRequestException("Mã OTP không chính xác hoặc đã hết hạn.");
            }

            var otpVerification = await _userRepository.GetLatestOtpVerificationWithUserAsync(request.Email, cancellationToken);
            if (otpVerification == null || otpVerification.Purpose != Domain.Enums.OtpPurpose.ForgotPassword)
            {
                throw new NotFoundException("Không tìm thấy thông tin xác thực quên mật khẩu cho tài khoản này.");
            }

            if (otpVerification.IsUsed)
            {
                throw new BadRequestException("Mã OTP này đã được sử dụng.");
            }

            if (otpVerification.ExpiredAt < DateTime.UtcNow)
            {
                throw new BadRequestException("Mã OTP này đã hết hạn.");
            }

            var user = otpVerification.User;
            if (user == null)
            {
                throw new NotFoundException("Tài khoản không tồn tại trên hệ thống.");
            }

            var hashedPassword = _passwordHasher.Hash(request.NewPassword);
            user.PasswordHash = hashedPassword;
            otpVerification.IsUsed = true;

            await _userRepository.UpdateUserAsync(user, cancellationToken);
            await _userRepository.SaveChangesAsync(cancellationToken);
            await _redisCacheService.DeleteAsync(redisKey, cancellationToken);

            return new VerifyChangePasswordResponse
            {
                Message = "Thay đổi mật khẩu thành công."
            };
        }
    }
}
