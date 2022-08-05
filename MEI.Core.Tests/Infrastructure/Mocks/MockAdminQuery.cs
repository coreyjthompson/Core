using MEI.Core.Infrastructure.Queries;

// keep .Admin in namespace as it is used in testing
// ReSharper disable once CheckNamespace
namespace MEI.Core.Tests.Infrastructure.Mocks
{
    public class MockAdminQuery
        : IQuery<MockResult>, ICacheQuery
    {
        public CacheQueryOptions CacheQueryOptions { get; set; }
    }
}
