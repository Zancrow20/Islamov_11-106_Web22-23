using System.Collections.Concurrent;
using HttpServer.Models;
using Microsoft.Extensions.Caching.Memory;
namespace HttpServer;

public class SessionManager
{
    private MemoryCache _cache = new MemoryCache(new MemoryCacheOptions());
    private ConcurrentDictionary<object, SemaphoreSlim> _locks = new ();
 
    public Session GetOrAdd(object key, Func<Session> createItem)
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

    public bool CheckSession(object key)
    {
        return _cache.TryGetValue(key, out _);
    }

    public Session? GetInfo(object key)
    {
        return _cache.TryGetValue(key, out Session session) ? session 
            : throw new KeyNotFoundException("Couldn't find this key");
    }
}