using System;

namespace WebAPI.Infrastructure.Services
{
    public interface ICacheService
    {
        TOuput GetFromCache<TOuput>(string cacheKey, Func<TOuput> func);
    }
}