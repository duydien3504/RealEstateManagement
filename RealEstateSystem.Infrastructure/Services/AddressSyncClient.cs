using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using RealEstateSystem.Application.DTOs.Response;
using RealEstateSystem.Application.Interfaces;

namespace RealEstateSystem.Infrastructure.Services
{
    public class AddressSyncClient : IAddressSyncClient
    {
        private readonly HttpClient _httpClient;

        public AddressSyncClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<ProvinceSyncDto>> FetchProvincesAndWardsAsync(CancellationToken cancellationToken)
        {
            var response = await _httpClient.GetFromJsonAsync<List<ProvinceSyncDto>>("https://provinces.open-api.vn/api/v2/?depth=2", cancellationToken);
            return response ?? new List<ProvinceSyncDto>();
        }
    }
}
