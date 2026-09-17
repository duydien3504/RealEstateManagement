using Microsoft.Extensions.DependencyInjection;
using RealEstateSystem.Application.Interfaces;
using RealEstateSystem.Application.Services;
using RealEstateSystem.Application.Services.AuthenService;
using RealEstateSystem.Application.Services.AdminPropertyService;
using RealEstateSystem.Application.Services.AdminUserService;
using RealEstateSystem.Application.Services.ChatService;
using RealEstateSystem.Application.Services.LocationService;
using RealEstateSystem.Application.Services.ProfileService;
using RealEstateSystem.Application.Services.PropertyReportService;
using RealEstateSystem.Application.Services.PropertyService;
using RealEstateSystem.Application.Services.StatisticsService;
using RealEstateSystem.Application.Services.WalletService;

namespace RealEstateSystem.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<RegisterService>();
            services.AddScoped<LoginService>();
            services.AddScoped<VerifyOtpService>();
            services.AddScoped<ForgetPasswordService>();
            services.AddScoped<VerifyChangePasswordService>();
            services.AddScoped<ChangePasswordService>();
            services.AddScoped<IAuthenService, AuthenService>();

            services.AddScoped<GetProfileService>();
            services.AddScoped<UpdateProfileService>();
            services.AddScoped<DeleteProfileService>();
            services.AddScoped<UploadAvatarService>();
            services.AddScoped<UpRoleService>();
            services.AddScoped<IProfileService, ProfileService>();

            services.AddScoped<IWalletService, PaymentWalletService>();
            services.AddScoped<IAddressService, AddressService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IAmenityService, AmenityService>();
            services.AddScoped<IPropertyService, PropertyService>();
            services.AddScoped<IChatService, ChatService>();
            services.AddScoped<IPropertyReportService, PropertyReportService>();
            services.AddScoped<IAdminUserService, AdminUserService>();
            services.AddScoped<IAdminPropertyService, AdminPropertyService>();
            services.AddScoped<IStatisticsService, StatisticsService>();

            return services;
        }
    }
}
