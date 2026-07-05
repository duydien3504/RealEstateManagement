using RealEstateSystem.Application.DTOs.Request;
using RealEstateSystem.Application.DTOs.Response;

namespace RealEstateSystem.Application.Interfaces
{
    public interface IAuthenService
    {
        Task<RegisterResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken);
        Task<LoginResponse> LoginAsync(LoginRequest request, string ipAddress, CancellationToken cancellationToken);
        Task<VerifyOtpResponse> VerifyOtpAsync(VerifyOtpRequest request, string otp, CancellationToken cancellationToken);
        Task<ForgetPasswordResponse> ForgetPasswordAsync(ForgetPasswordRequest request, CancellationToken cancellationToken);
        Task<VerifyChangePasswordResponse> VerifyChangePasswordAsync(VerifyChangePasswordRequest request, string otp, CancellationToken cancellationToken);
        Task<ChangePasswordResponse> ChangePasswordAsync(Guid userId, ChangePasswordRequest request, CancellationToken cancellationToken);
    }
}
