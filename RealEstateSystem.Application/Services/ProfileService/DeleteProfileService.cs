using RealEstateSystem.Application.DTOs.Request;
using RealEstateSystem.Application.DTOs.Response;
using RealEstateSystem.Application.Interfaces;

namespace RealEstateSystem.Application.Services.ProfileService
{
    public class DeleteProfileService
    {
        private readonly IUserRepository _userRepository;
        private readonly IEncryptEmail _encryptEmail;

        public DeleteProfileService(IUserRepository userRepository, IEncryptEmail encryptEmail)
        {
            _userRepository = userRepository;
            _encryptEmail = encryptEmail;
        }

        public async Task<DeleteProfileResponse> DeleteProfileAsync(Guid userId, DeleteProfileRequest request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserByIdAsync(userId, cancellationToken);
            if (user == null)
            {
                throw new ArgumentException("Tài khoản không tồn tại.");
            }

            if (!string.Equals(user.Email, request.Email, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Email không khớp với tài khoản đang đăng nhập.");
            }

            user.IsDeleted = true;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateUserAsync(user, cancellationToken);
            await _userRepository.SaveChangesAsync(cancellationToken);

            var encryptedEmail = _encryptEmail.Encrypt(request.Email);

            return new DeleteProfileResponse
            {
                Message = "Yêu cầu xóa tài khoản đã được xử lý thành công.",
                EncryptedEmail = encryptedEmail
            };
        }
    }
}
