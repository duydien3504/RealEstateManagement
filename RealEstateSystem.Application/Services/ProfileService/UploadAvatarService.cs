using Microsoft.Extensions.Logging;
using RealEstateSystem.Application.DTOs.Response;
using RealEstateSystem.Application.Interfaces;

namespace RealEstateSystem.Application.Services.ProfileService
{
    public class UploadAvatarService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUploadCloud _uploadCloud;
        private readonly IRedisCacheService _redisCacheService;
        private readonly ILogger<UploadAvatarService> _logger;

        public UploadAvatarService(
            IUserRepository userRepository,
            IUploadCloud uploadCloud,
            IRedisCacheService redisCacheService,
            ILogger<UploadAvatarService> logger)
        {
            _userRepository = userRepository;
            _uploadCloud = uploadCloud;
            _redisCacheService = redisCacheService;
            _logger = logger;
        }

        public async Task<UploadAvatarResponse> UploadAvatarAsync(Guid userId, string tokenEmail, byte[] fileBytes, string fileName, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserByIdAsync(userId, cancellationToken);
            if (user == null)
            {
                return new UploadAvatarResponse
                {
                    IsSuccess = false,
                    Message = "Người dùng không tồn tại."
                };
            }

            if (!string.Equals(user.Email, tokenEmail, StringComparison.OrdinalIgnoreCase))
            {
                return new UploadAvatarResponse
                {
                    IsSuccess = false,
                    Message = "Email không khớp với tài khoản."
                };
            }

            int attempt = 0;
            string? avatarUrl = null;
            var cacheKey = $"upload_retry:{userId}";

            while (attempt < 3)
            {
                try
                {
                    avatarUrl = await _uploadCloud.UploadImageAsync(fileBytes, fileName, "user-avatars", cancellationToken);
                    break;
                }
                catch (Exception ex)
                {
                    attempt++;
                    await _redisCacheService.SetAsync(cacheKey, attempt.ToString(), TimeSpan.FromMinutes(5), cancellationToken);

                    if (attempt >= 3)
                     {
                        _logger.LogError(ex, "Upload ảnh đại diện cho người dùng {UserId} thất bại sau 3 lần thử.", userId);
                        await _redisCacheService.DeleteAsync(cacheKey, cancellationToken);
                        return new UploadAvatarResponse
                        {
                            IsSuccess = false,
                            Message = "Upload ảnh đại diện thất bại sau 3 lần thử."
                        };
                    }

                    await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
                }
            }

            await _redisCacheService.DeleteAsync(cacheKey, cancellationToken);

            user.AvatarUrl = avatarUrl;
            await _userRepository.UpdateUserAsync(user, cancellationToken);
            await _userRepository.SaveChangesAsync(cancellationToken);

            return new UploadAvatarResponse
            {
                IsSuccess = true,
                Message = "Cập nhật ảnh đại diện thành công.",
                AvatarUrl = avatarUrl
            };
        }
    }
}
