using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RealEstateSystem.Domain.Entity;

namespace RealEstateSystem.Application.Interfaces
{
    public interface IAmenityRepository
    {
        Task<List<Amenity>> GetAllAsync(CancellationToken cancellationToken);
    }
}
