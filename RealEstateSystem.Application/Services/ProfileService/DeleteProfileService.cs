using Microsoft.Extensions.Logging;
using RealEstateSystem.Application.DTOs.Request;
using RealEstateSystem.Application.DTOs.Response;
using RealEstateSystem.Application.Interfaces;
using RealEstateSystem.Domain.Exceptions;

namespace RealEstateSystem.Application.Services.ProfileService
{
    public class DeleteProfileService
    {
        private readonly IUserRepository _userRepository;
        private readonly IEncryptEmail _encryptEmail;
        private readonly ILogger<DeleteProfileService> _logger;

        public DeleteProfileService(
            IUserRepository userRepository, 
            IEncryptEmail encryptEmail,
            ILogger<DeleteProfileService> logger)
        {
            _userRepository = userRepository;
            _encryptEmail = encryptEmail;
            _logger = logger;
        }

        public async Task<DeleteProfileResponse> DeleteProfileAsync(Guid userId, DeleteProfileRequest request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserByIdAsync(userId, cancellationToken);
            if (user == null)
            {
                _logger.LogWarning("Xóa tài khoản thất bại: Không tìm thấy người dùng có ID {UserId}.", userId);
                throw new NotFoundException("Tài khoản không tồn tại.");
            }

            if (!string.Equals(user.Email, request.Email, StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("Xóa tài khoản thất bại: Email nhập vào '{InputEmail}' không khớp với email trong DB '{UserEmail}' của ID {UserId}.", request.Email, user.Email, userId);
                throw new BadRequestException("Email không khớp với tài khoản đang đăng nhập.");
            }

            user.IsDeleted = true;

            await _userRepository.UpdateUserAsync(user, cancellationToken);
            await _userRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Người dùng {Email} (ID: {UserId}) đã được xóa mềm thành công.", user.Email, user.UserId);

            var encryptedEmail = _encryptEmail.Encrypt(request.Email);

            return new DeleteProfileResponse
            {
                Message = "Yêu cầu xóa tài khoản đã được xử lý thành công.",
                EncryptedEmail = encryptedEmail
            };
        }
    }
}

