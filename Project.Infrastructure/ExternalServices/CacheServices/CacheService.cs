using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Project.Application.Common.Interfaces.IExternalServices.ICacheServices;

namespace Project.Infrastructure.ExternalServices.CacheServices
{
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<CacheService> _logger;

        public CacheService(IMemoryCache cache, ILogger<CacheService> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public T Get<T>(string key)
        {
            bool hit = _cache.TryGetValue(key, out T? value);

            if (hit)
                _logger.LogDebug("[Cache HIT] Key: {Key}, Type: {Type}", key, typeof(T).Name);
            else
                _logger.LogDebug("[Cache MISS] Key: {Key}, Type: {Type}", key, typeof(T).Name);

            return value ?? default!;
        }

        public void Set<T>(string key, T value, TimeSpan? expiration = null)
        {
            if (expiration.HasValue)
            {
                _cache.Set(key, value, expiration.Value);
                _logger.LogDebug("[Cache SET] Key: {Key}, Type: {Type}, Expiration: {Expiration}",
                    key, typeof(T).Name, expiration.Value);
            }
            else
            {
                _cache.Set(key, value);
                _logger.LogDebug("[Cache SET] Key: {Key}, Type: {Type}, Expiration: none",
                    key, typeof(T).Name);
            }
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
            _logger.LogDebug("[Cache REMOVE] Key: {Key}", key);
        }

        public bool Exists(string key)
        {
            var exists = _cache.TryGetValue(key, out _);

            return exists;
        }
    }
}