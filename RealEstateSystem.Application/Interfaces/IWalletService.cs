using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RealEstateSystem.Application.DTOs.Request;
using RealEstateSystem.Application.DTOs.Response;

namespace RealEstateSystem.Application.Interfaces
{
    public interface IWalletService
    {
        Task<TopUpResponse> TopUpWalletAsync(Guid userId, string email, TopUpRequest request, string ipAddress, CancellationToken cancellationToken);
        Task<VnpayReturnResponse> ProcessReturnUrlAsync(Dictionary<string, string> queryParameters, CancellationToken cancellationToken);
        Task<VnpayIpnResponse> ProcessIpnAsync(Dictionary<string, string> queryParameters, CancellationToken cancellationToken);
        Task<WalletDetailsResponse> GetWalletDetailsAsync(Guid userId, int pageNumber, int pageSize, CancellationToken cancellationToken);
    }
}
