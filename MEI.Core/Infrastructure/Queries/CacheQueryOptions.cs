using System;

namespace MEI.Core.Infrastructure.Queries
{
    public class CacheQueryOptions
    {
        public CacheQueryOptions()
        {
            Size = 1;
        }

        public string CacheKey { get; set; }

        public TimeSpan? SlidingExpiration { get; set; }

        public DateTimeOffset? AbsoluteExpiration { get; set; }

        public long Size { get; set; }
    }
}