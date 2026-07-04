using Microsoft.EntityFrameworkCore;
using RealEstateSystem.Infrastructure.Data;

namespace RealEstateSystem.API.Extensions
{
    public static class DatabaseExtentsion
    {
        public static IServiceCollection AddSqlServer(this IServiceCollection services, IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("DefaultConnections");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Khong tim thay chuoi ket noi");
            }

            services.AddDbContext<RealEstateDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });

            return services;
        }
    }
}
