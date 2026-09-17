using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RealEstateSystem.Application.DTOs.Response;

namespace RealEstateSystem.Application.Interfaces
{
    public interface IAddressSyncClient
    {
        Task<List<ProvinceSyncDto>> FetchProvincesAndWardsAsync(CancellationToken cancellationToken);
    }
}
