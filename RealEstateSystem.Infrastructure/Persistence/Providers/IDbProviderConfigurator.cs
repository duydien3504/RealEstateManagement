using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace RealEstateSystem.Application.Interfaces
{
    public interface IDbProviderConfigurator
    {
        string ProviderName { get; }
        void Configure(DbContextOptionsBuilder options, string connectionString);
    }
}
