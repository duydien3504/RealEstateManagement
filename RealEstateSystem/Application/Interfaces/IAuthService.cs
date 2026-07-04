using RealEstateSystem.Application.DTOs.Request;
using RealEstateSystem.Application.DTOs.Response;

namespace RealEstateSystem.Application.Interfaces
{
    public interface IAuthService
    {
        Task<RegisterResponse> RegisterAsync(RegisterRequest request);
        Task<LoginResponse> LoginAsync(LoginRequest request);
    }
}
