using Microsoft.EntityFrameworkCore;
using RealEstateSystem.Infrastructure.Persistence;

namespace RealEstateSystem.Api.Extensions
{
    public static class DatabaseExtensions
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnections");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("Chuỗi kết nối rỗng");
            }

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            return services;
        }
    }
}
