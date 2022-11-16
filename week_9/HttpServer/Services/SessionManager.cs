using System.Collections.Concurrent;
using HttpServer.Models;
using Microsoft.Extensions.Caching.Memory;
namespace HttpServer;

public class SessionManager
{
    private static MemoryCache _cache = new MemoryCache(new MemoryCacheOptions());
    private static ConcurrentDictionary<object, SemaphoreSlim> _locks = new ();
 
    public static Session GetOrAdd(object key, Func<Session> createItem)
    {
        if (!_cache.TryGetValue(key, out Session cacheEntry))
        {
            SemaphoreSlim slim = _locks.GetOrAdd(key, k => new SemaphoreSlim(1, 1));
            
            slim.WaitAsync();
            try
            {
                if (!_cache.TryGetValue(key, out cacheEntry))
                {
                    cacheEntry = createItem();
                    var cacheEntryOptions =
                        new MemoryCacheEntryOptions()
                            .SetSlidingExpiration(TimeSpan.FromMinutes(2))
                            .SetAbsoluteExpiration(TimeSpan.FromMinutes(2));
                    _cache.Set(key, cacheEntry, cacheEntryOptions);
                }
            }
            finally
            {
                slim.Release();
            }
        }
        return cacheEntry;
    }

    public static bool CheckSession(object key)
    {
        var contains = _cache.TryGetValue(key, out Session session);
        return contains ? contains : throw new KeyNotFoundException("Couldn't find this key");
    }

    public static Session? GetInfo(object key)
    {
        return _cache.Get<Session>(key);

    }
}