using Microsoft.EntityFrameworkCore;
using RealEstateSystem.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstateSystem.Infrastructure.Persistence.Providers
{
    public class MySqlProviderConfigurator : IDbProviderConfigurator
    {
        public string ProviderName => "Mysql";

        public void Configure(DbContextOptionsBuilder options, string connectionString)
        {
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), b => b.MigrationsAssembly("RealEstateSystem.Infrastructure"));
        }
    }
}
