using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using LazyCache;
using LazyCache.Mocks;

using Microsoft.Extensions.Caching.Memory;

namespace MEI.Core.Tests.Infrastructure.Mocks
{
    public class MockAppCache
        : IAppCache
    {
        public MockAppCache()
        {
            TheCache = new Dictionary<string, (object item, MemoryCacheEntryOptions options)>();
        }

        public Dictionary<string, (object item, MemoryCacheEntryOptions options)> TheCache { get; }

        public void Add<T>(string key, T item, MemoryCacheEntryOptions policy)
        {
            if (TheCache.ContainsKey(key))
            {
                return;
            }

            TheCache.Add(key, (item, policy));
        }

        public T Get<T>(string key)
        {
            if (TheCache.ContainsKey(key))
            {
                return (T)TheCache[key].item;
            }

            return default;
        }

        public T GetOrAdd<T>(string key, Func<ICacheEntry, T> addItemFactory)
        {
            if (TheCache.ContainsKey(key))
            {
                return (T)TheCache[key].item;
            }

            var entry = new MockCacheEntry(key);

            var value = addItemFactory(entry);

            TheCache.Add(key, (value, new MemoryCacheEntryOptions
                                      {
                                          AbsoluteExpiration = entry.AbsoluteExpiration,
                                          SlidingExpiration = entry.SlidingExpiration,
                                          AbsoluteExpirationRelativeToNow = entry.AbsoluteExpirationRelativeToNow,
                                          Priority = entry.Priority,
                                          Size = entry.Size
                                      }));

            return value;
        }

        public Task<T> GetAsync<T>(string key)
        {
            if (TheCache.ContainsKey(key))
            {
                return Task.FromResult((T)TheCache[key].item);
            }

            return Task.FromResult(default(T));
        }

        public Task<T> GetOrAddAsync<T>(string key, Func<ICacheEntry, Task<T>> addItemFactory)
        {
            if (TheCache.ContainsKey(key))
            {
                return Task.FromResult((T)TheCache[key].item);
            }

            var entry = new MockCacheEntry(key);

            var value = addItemFactory(entry).Result;

            TheCache.Add(key, (value, new MemoryCacheEntryOptions
                                      {
                                          AbsoluteExpiration = entry.AbsoluteExpiration,
                                          SlidingExpiration = entry.SlidingExpiration,
                                          AbsoluteExpirationRelativeToNow = entry.AbsoluteExpirationRelativeToNow,
                                          Priority = entry.Priority,
                                          Size = entry.Size
                                      }));

            return Task.FromResult(value);
        }

        public void Remove(string key)
        {
            TheCache.Remove(key);
        }

        public ICacheProvider CacheProvider => new MockCacheProvider();
        public CacheDefaults DefaultCachePolicy => new CacheDefaults();
    }
}
