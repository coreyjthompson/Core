using System;
using System.Runtime.Caching;

using NodaTime;
using NodaTime.Extensions;

namespace MEI.SPDocuments.Data
{
    public interface ICacheProvider
    {
        T GetCachedData<T>(string cacheKey, object cacheLock, int cacheTimePolicyMinutes, Func<T> GetData)
            where T : class;

        T GetCachedDataAsValue<T>(string cacheKey, object cacheLock, int cacheTimePolicyMinutes, Func<T> GetData)
            where T : struct;

        T GetCachedData<T>(string cacheKey)
            where T : class;

        bool IsSet(string cacheKey);

        void Invalidate(string cacheKey);
    }

    public class DefaultCacheProvider
        : ICacheProvider
    {
        private readonly IClock _clock;

        public DefaultCacheProvider(IClock clock)
        {
            _clock = clock;
        }

        public T GetCachedData<T>(string cacheKey, object cacheLock, int cacheTimePolicyMinutes, Func<T> GetData)
            where T : class
        {
            // Makes sure that the key is always formed the same no matter what
            cacheKey = (cacheKey ?? string.Empty).ToLower();

            string cacheSavedDateTimeKey = cacheKey + "_saveddatetime";

            // Returns null if the string does not exist, prevents a race condition where the cache invalidates
            // between the contains check and the retrieval.

            if (MemoryCache.Default.Get(cacheKey) is T cachedData)
            {
                return cachedData;
            }

            ZonedDateTime now = _clock.InTzdbSystemDefaultZone().GetCurrentZonedDateTime();

            lock (cacheLock)
            {
                // Check to see if anyone wrote to the cache while we were waiting our turn to write the new value.
                cachedData = MemoryCache.Default.Get(cacheKey) as T;

                if (cachedData != null)
                {
                    return cachedData;
                }

                // The value still did not exist so we now write it in to the cache.
                var cachePolicy = new CacheItemPolicy
                                  {
                                      AbsoluteExpiration = now.Plus(Duration.FromMinutes(cacheTimePolicyMinutes)).ToDateTimeOffset()
                                  };

                cachedData = GetData();
                MemoryCache.Default.Set(cacheKey, cachedData, cachePolicy);

                // Cache the datetime that this was saved
                MemoryCache.Default.Set(cacheSavedDateTimeKey, DateTime.Now.ToString("MM/dd/yyyy hh:mm tt"), cachePolicy);

                return cachedData;
            }
        }

        public T GetCachedDataAsValue<T>(string cacheKey, object cacheLock, int cacheTimePolicyMinutes, Func<T> GetData)
            where T : struct
        {
            // Makes sure that the key is always formed the same no matter what
            cacheKey = (cacheKey ?? string.Empty).ToLower();

            object cachedData = MemoryCache.Default.Get(cacheKey);

            if (cachedData != null)
            {
                return (T) cachedData;
            }

            lock (cacheLock)
            {
                cachedData = MemoryCache.Default.Get(cacheKey);
            }

            if (cachedData != null)
            {
                return (T) cachedData;
            }

            ZonedDateTime now = _clock.InTzdbSystemDefaultZone().GetCurrentZonedDateTime();

            var cachePolicy = new CacheItemPolicy
                              {
                                  AbsoluteExpiration = now.Plus(Duration.FromMinutes(cacheTimePolicyMinutes)).ToDateTimeOffset()
                              };

            cachedData = GetData();
            MemoryCache.Default.Set(cacheKey, cachedData, cachePolicy);

            return (T) cachedData;
        }

        public T GetCachedData<T>(string cacheKey)
            where T : class
        {
            // Makes sure that the key is always formed the same no matter what
            cacheKey = (cacheKey ?? string.Empty).ToLower();

            // Returns null if the string does not exist, prevents a race condition where the cache invalidates
            // between the contains check and the retrieval.
            return MemoryCache.Default.Get(cacheKey) as T;
        }

        public bool IsSet(string cacheKey)
        {
            // Makes sure that the key is always formed the same no matter what
            cacheKey = (cacheKey ?? string.Empty).ToLower();

            return MemoryCache.Default.Get(cacheKey) != null;
        }

        public void Invalidate(string cacheKey)
        {
            // Makes sure that the key is always formed the same no matter what
            cacheKey = (cacheKey ?? string.Empty).ToLower();

            MemoryCache.Default.Remove(cacheKey);
        }
    }
}