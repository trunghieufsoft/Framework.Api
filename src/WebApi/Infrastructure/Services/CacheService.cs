using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using Common.Core.Timing;

namespace WebAPI.Infrastructure.Services
{
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _cache;
        private readonly IConfiguration _configuration;

        public CacheService(
            IMemoryCache cache,
            IConfiguration configuration)
        {
            _cache = cache;
            _configuration = configuration;
        }

        public TOuput GetFromCache<TOuput>(string cacheKey, Func<TOuput> func)
        {
            if (_cache.TryGetValue(cacheKey, out TOuput cacheEntry))
            {
                return cacheEntry;
            }

            cacheEntry = func();

            _cache.Set(
                cacheKey,
                cacheEntry,
                Clock.Now.AddDays(double.Parse(_configuration["Config:CacheMasterDataInDays"])));

            return cacheEntry;
        }
    }
}