using Microsoft.Extensions.Configuration;
using RealEstateSystem.Application.Interfaces;
using StackExchange.Redis;

namespace RealEstateSystem.Infrastructure.Services
{
    public class RedisCacheService : IRedisCacheService
    {
        private readonly IDatabase _redisDatabase;

        public RedisCacheService(IConnectionMultiplexer connectionMultiplexer)
        {
            _redisDatabase = connectionMultiplexer.GetDatabase();
        }

        public async Task SetAsync(string key, string value, TimeSpan expiry, CancellationToken cancellationToken)
        {
            await _redisDatabase.StringSetAsync(key, value, expiry);
        }

        public async Task<string?> GetAsync(string key, CancellationToken cancellationToken)
        {
            var value = await _redisDatabase.StringGetAsync(key);
            return value.HasValue ? value.ToString() : null;
        }

        public async Task DeleteAsync(string key, CancellationToken cancellationToken)
        {
            await _redisDatabase.KeyDeleteAsync(key);
        }
    }
}
