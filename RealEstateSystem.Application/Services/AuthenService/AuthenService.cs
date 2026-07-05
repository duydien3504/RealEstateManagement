using RealEstateSystem.Application.DTOs.Request;
using RealEstateSystem.Application.DTOs.Response;
using RealEstateSystem.Application.Interfaces;

namespace RealEstateSystem.Application.Services.AuthenService
{
    public class AuthenService : IAuthenService
    {
        private readonly RegisterService _registerService;
        private readonly LoginService _loginService;
        private readonly VerifyOtpService _verifyOtpService;
        private readonly ForgetPasswordService _forgetPasswordService;
        private readonly VerifyChangePasswordService _verifyChangePasswordService;
        private readonly ChangePasswordService _changePasswordService;

        public AuthenService(
            RegisterService registerService,
            LoginService loginService,
            VerifyOtpService verifyOtpService,
            ForgetPasswordService forgetPasswordService,
            VerifyChangePasswordService verifyChangePasswordService,
            ChangePasswordService changePasswordService)
        {
            _registerService = registerService;
            _loginService = loginService;
            _verifyOtpService = verifyOtpService;
            _forgetPasswordService = forgetPasswordService;
            _verifyChangePasswordService = verifyChangePasswordService;
            _changePasswordService = changePasswordService;
        }

        public async Task<RegisterResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken)
        {
            return await _registerService.RegisterAsync(request, cancellationToken);
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request, string ipAddress, CancellationToken cancellationToken)
        {
            return await _loginService.LoginAsync(request, ipAddress, cancellationToken);
        }

        public async Task<VerifyOtpResponse> VerifyOtpAsync(VerifyOtpRequest request, string otp, CancellationToken cancellationToken)
        {
            return await _verifyOtpService.VerifyOtpAsync(request, otp, cancellationToken);
        }

        public async Task<ForgetPasswordResponse> ForgetPasswordAsync(ForgetPasswordRequest request, CancellationToken cancellationToken)
        {
            return await _forgetPasswordService.ForgetPasswordAsync(request, cancellationToken);
        }

        public async Task<VerifyChangePasswordResponse> VerifyChangePasswordAsync(VerifyChangePasswordRequest request, string otp, CancellationToken cancellationToken)
        {
            return await _verifyChangePasswordService.VerifyChangePasswordAsync(request, otp, cancellationToken);
        }

        public async Task<ChangePasswordResponse> ChangePasswordAsync(Guid userId, ChangePasswordRequest request, CancellationToken cancellationToken)
        {
            return await _changePasswordService.ChangePasswordAsync(userId, request, cancellationToken);
        }
    }
}
