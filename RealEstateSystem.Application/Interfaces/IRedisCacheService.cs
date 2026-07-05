namespace RealEstateSystem.Application.Interfaces
{
    public interface IRedisCacheService
    {
        Task SetAsync(string key, string value, TimeSpan expiry, CancellationToken cancellationToken);
        Task<string?> GetAsync(string key, CancellationToken cancellationToken);
        Task DeleteAsync(string key, CancellationToken cancellationToken);
    }
}
