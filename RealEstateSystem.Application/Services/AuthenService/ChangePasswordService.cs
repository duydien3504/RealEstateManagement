using Microsoft.Extensions.Logging;
using RealEstateSystem.Application.DTOs.Request;
using RealEstateSystem.Application.DTOs.Response;
using RealEstateSystem.Application.Interfaces;
using RealEstateSystem.Domain.Exceptions;

namespace RealEstateSystem.Application.Services.AuthenService
{
    public class ChangePasswordService
    {
        private readonly IUserRepository _userRepository;
        private readonly IHasherPassword _passwordHasher;
        private readonly ILogger<ChangePasswordService> _logger;

        public ChangePasswordService(
            IUserRepository userRepository, 
            IHasherPassword passwordHasher,
            ILogger<ChangePasswordService> logger)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _logger = logger;
        }

        public async Task<ChangePasswordResponse> ChangePasswordAsync(
            Guid userId,
            ChangePasswordRequest request,
            CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserByIdAsync(userId, cancellationToken);
            if (user == null)
            {
                _logger.LogWarning("Đổi mật khẩu thất bại: Tài khoản với ID {UserId} không tồn tại.", userId);
                throw new NotFoundException("Tài khoản không tồn tại.");
            }

            var isOldPasswordValid = _passwordHasher.Verify(request.OldPassword, user.PasswordHash);
            if (!isOldPasswordValid)
            {
                _logger.LogWarning("Đổi mật khẩu thất bại: Người dùng {Email} nhập sai mật khẩu cũ.", user.Email);
                throw new BadRequestException("Mật khẩu cũ không chính xác.");
            }

            var hashedNewPassword = _passwordHasher.Hash(request.NewPassword);
            user.PasswordHash = hashedNewPassword;

            await _userRepository.UpdateUserAsync(user, cancellationToken);
            await _userRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Người dùng {Email} (ID: {UserId}) đã đổi mật khẩu thành công.", user.Email, user.UserId);

            return new ChangePasswordResponse
            {
                Message = "Thay đổi mật khẩu thành công."
            };
        }
    }
}

