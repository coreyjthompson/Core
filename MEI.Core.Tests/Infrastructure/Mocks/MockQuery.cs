using MEI.Core.Infrastructure.Queries;

namespace MEI.Core.Tests.Infrastructure.Mocks
{
    public class MockQuery
        : IQuery<MockResult>, ICacheQuery
    {
        public CacheQueryOptions CacheQueryOptions { get; set; }

        public override string ToString()
        {
            return "[MockQuery]";
        }
    }
}
