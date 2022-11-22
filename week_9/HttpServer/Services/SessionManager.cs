using System.Collections.Concurrent;
using HttpServer.Models;
using Microsoft.Extensions.Caching.Memory;
namespace HttpServer;

public class SessionManager
{
    private static MemoryCache _cache = new MemoryCache(new MemoryCacheOptions());
    private static ConcurrentDictionary<object, SemaphoreSlim> _locks = new ();
 
    public static async Task<Session> GetOrAdd(object key, Func<Task<Session>> createItem)
    {
        if (!_cache.TryGetValue(key, out Session cacheEntry))
        {
            SemaphoreSlim mylock = _locks.GetOrAdd(key, k => new SemaphoreSlim(1, 1));
 
            await mylock.WaitAsync();
            try
            {
                if (!_cache.TryGetValue(key, out cacheEntry))
                {
                    cacheEntry = await createItem();
                    var cacheEntryOptions =
                        new MemoryCacheEntryOptions()
                            .SetSlidingExpiration(TimeSpan.FromMinutes(2000))
                            .SetAbsoluteExpiration(TimeSpan.FromMinutes(2000));
                    _cache.Set(key, cacheEntry, cacheEntryOptions);
                }
            }
            finally
            {
                mylock.Release();
            }
        }
        return cacheEntry;
    }

    public static async Task<bool> CheckSession(object key)
    {
        var contains = _cache.TryGetValue(key, out Session session);
        return await (contains ? Task.FromResult(contains) : throw new KeyNotFoundException("Couldn't find this key"));
    }

    public static async Task<Session?> GetInfo(object key)
    {
        return await Task.FromResult(_cache.Get<Session>(key));
    }
}