using Microsoft.EntityFrameworkCore;
using RealEstateSystem.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstateSystem.Infrastructure.Persistence.Providers
{
    public class SqlServerProviderConfigurator : IDbProviderConfigurator
    {
        public string ProviderName => "Sqlserver";

        public void Configure(DbContextOptionsBuilder options, string connectionString)
        {
            options.UseSqlServer(connectionString, b => b.MigrationsAssembly("RealEstateSystem.Infrastructure"));
        }
    }
}
