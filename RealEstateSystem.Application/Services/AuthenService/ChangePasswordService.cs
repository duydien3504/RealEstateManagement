using RealEstateSystem.Application.DTOs.Request;
using RealEstateSystem.Application.DTOs.Response;
using RealEstateSystem.Application.Interfaces;

namespace RealEstateSystem.Application.Services.AuthenService
{
    public class ChangePasswordService
    {
        private readonly IUserRepository _userRepository;
        private readonly IHasherPassword _passwordHasher;

        public ChangePasswordService(IUserRepository userRepository, IHasherPassword passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<ChangePasswordResponse> ChangePasswordAsync(
            Guid userId,
            ChangePasswordRequest request,
            CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserByIdAsync(userId, cancellationToken);
            if (user == null)
            {
                throw new ArgumentException("Tài khoản không tồn tại.");
            }

            var isOldPasswordValid = _passwordHasher.Verify(request.OldPassword, user.PasswordHash);
            if (!isOldPasswordValid)
            {
                throw new ArgumentException("Mật khẩu cũ không chính xác.");
            }

            var hashedNewPassword = _passwordHasher.Hash(request.NewPassword);
            user.PasswordHash = hashedNewPassword;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateUserAsync(user, cancellationToken);
            await _userRepository.SaveChangesAsync(cancellationToken);

            return new ChangePasswordResponse
            {
                Message = "Thay đổi mật khẩu thành công."
            };
        }
    }
}
