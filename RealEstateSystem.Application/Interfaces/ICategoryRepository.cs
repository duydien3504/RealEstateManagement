using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RealEstateSystem.Domain.Entity;

namespace RealEstateSystem.Application.Interfaces
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetAllAsync(CancellationToken cancellationToken);
    }
}
