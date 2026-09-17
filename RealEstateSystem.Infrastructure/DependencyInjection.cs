using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RealEstateSystem.Application.Interfaces;

using RealEstateSystem.Infrastructure.Configuration;
using RealEstateSystem.Infrastructure.Messaging;
using RealEstateSystem.Infrastructure.Persistence;
using RealEstateSystem.Infrastructure.Persistence.Providers;
using RealEstateSystem.Infrastructure.Repository;
using RealEstateSystem.Infrastructure.Security;
using RealEstateSystem.Infrastructure.Services;
using StackExchange.Redis;

namespace RealEstateSystem.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            var providerName = configuration["DatabaseSettings:Provider"];
            var connectionString = configuration["DatabaseSettings:DefaultConnections"];
            
            if(string.IsNullOrEmpty(providerName) || string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Cấu hình database thiếu thông tin");
            }

            services.AddSingleton<IDbProviderConfigurator, SqlServerProviderConfigurator>();
            services.AddSingleton<IDbProviderConfigurator, PostgreSqlProviderConfigurator>();
            services.AddSingleton<IDbProviderConfigurator, MySqlProviderConfigurator>();

            services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
            {
                var configurators = serviceProvider.GetServices<IDbProviderConfigurator>();
                var configuratorRegistry = configurators.ToDictionary(c => c.ProviderName, StringComparer.OrdinalIgnoreCase);

                if (configuratorRegistry.TryGetValue(providerName, out var configurator))
                {
                    configurator.Configure(options, connectionString);
                }
                else
                {
                    throw new NotSupportedException($"Hệ quản trị CSDL '{providerName}' không được hỗ trợ.");
                }
            });

            var redisConnectionString = configuration["Redis:ConnectionString"]
                ?? throw new InvalidOperationException("Cấu hình Redis:ConnectionString không tồn tại.");
            services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnectionString));

            var rabbitHost = configuration["RabbitMq:Host"] ?? "localhost";
            var rabbitPort = int.Parse(configuration["RabbitMq:Port"] ?? "5672");
            var rabbitUsername = configuration["RabbitMq:Username"] ?? "guest";
            var rabbitPassword = configuration["RabbitMq:Password"] ?? "guest";

            services.AddSingleton<IConnection>(serviceProvider =>
            {
                var factory = new ConnectionFactory
                {
                    HostName = rabbitHost,
                    Port = rabbitPort,
                    UserName = rabbitUsername,
                    Password = rabbitPassword
                };
                return factory.CreateConnectionAsync().GetAwaiter().GetResult();
            });

            services.AddScoped<IHasherPassword, PasswordHasher>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IWalletRepository, WalletRepository>();
            services.AddScoped<IRedisCacheService, RedisCacheService>();
            services.AddScoped<IMessagePublisher, RabbitMqPublisher>();
            services.AddTransient<IMailService, SmtpMailService>();
            services.AddScoped<IEncryptEmail, HmacEncrypt>();
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
            services.AddScoped<IUploadCloud, CloudinaryService>();
            services.AddScoped<IPayment, VnpayPaymentService>();
            services.AddScoped<IAddressRepository, AddressRepository>();
            services.AddScoped<System.Net.Http.HttpClient>();
            services.AddScoped<IAddressSyncClient, AddressSyncClient>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IAmenityRepository, AmenityRepository>();
            services.AddScoped<IPropertyRepository, PropertyRepository>();
            services.AddScoped<IFavoriteRepository, FavoriteRepository>();
            services.AddScoped<IFavoriteService, RealEstateSystem.Application.Services.FavoriteService.FavoriteService>();
            services.AddScoped<IAppointmentRepository, AppointmentRepository>();
            services.AddScoped<IAppointmentService, RealEstateSystem.Application.Services.AppointmentService.AppointmentService>();
            services.AddScoped<IChatRepository, ChatRepository>();
            services.AddScoped<IPropertyReportRepository, PropertyReportRepository>();
            services.AddScoped<IStatisticsRepository, StatisticsRepository>();

            services.Configure<GeminiSettings>(options =>
            {
                options.ApiKey = configuration["Gemini:ApiKey"] ?? string.Empty;
                options.Model = configuration["Gemini:Model"] ?? string.Empty;
            });
            services.AddScoped<IAiService, GeminiAiService>();

            return services;
        }
    }
}
