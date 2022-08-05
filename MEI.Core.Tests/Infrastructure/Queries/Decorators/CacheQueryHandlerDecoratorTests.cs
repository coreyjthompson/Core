using System;
using System.Threading.Tasks;

using MEI.Core.Infrastructure.Queries;
using MEI.Core.Infrastructure.Queries.Decorators;
using MEI.Core.Tests.Infrastructure.Mocks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

namespace MEI.Core.Tests.Infrastructure.Queries.Decorators
{
    [TestClass]
    public class CacheQueryHandlerDecoratorTests
    {
        private CacheQueryHandlerDecorator<MockQuery, MockResult> _target;

        private MockAppCache _cache;
        private Mock<IQueryHandler<MockQuery, MockResult>> _queryHandler;

        [TestInitialize]
        public void Initialize()
        {
            _cache = new MockAppCache();
            _queryHandler = new Mock<IQueryHandler<MockQuery, MockResult>>();

            _target = new CacheQueryHandlerDecorator<MockQuery, MockResult>(_queryHandler.Object, _cache);
        }

        [TestMethod]
        public async Task HandleAsync_IsNotCacheQuery_DoNotUseCache()
        {
            var queryHandler = new Mock<IQueryHandler<MockNonCacheQuery, MockResult>>();
            var target = new CacheQueryHandlerDecorator<MockNonCacheQuery, MockResult>(queryHandler.Object, _cache);
            var query = new MockNonCacheQuery();

            await target.HandleAsync(query);

            queryHandler.Verify(x => x.HandleAsync(query), Times.Once);
        }

        [TestMethod]
        public async Task HandleAsync_IsNotCached_UseCache()
        {
            var key = "test";
            var query = new MockQuery
                        {
                            CacheQueryOptions = new CacheQueryOptions
                                                {
                                                    CacheKey = key
                                                }
                        };
            var result = new MockResult();
            _queryHandler.Setup(x => x.HandleAsync(query)).Returns(Task.FromResult(result));

            await _target.HandleAsync(query);

            Assert.IsTrue(_cache.TheCache.ContainsKey(key));
            Assert.AreEqual(result, _cache.TheCache[key].item);
        }

        [TestMethod]
        public async Task HandleAsync_NullCacheQueryOptions_ThrowException()
        {
            var query = new MockQuery
                        {
                            CacheQueryOptions = null
                        };

            await Assert.ThrowsExceptionAsync<ArgumentException>(() => _target.HandleAsync(query));
        }

        [TestMethod]
        public async Task HandleAsync_NullCacheKey_ThrowException()
        {
            var query = new MockQuery
                        {
                            CacheQueryOptions = new CacheQueryOptions
                                                {
                                                    CacheKey = null
                                                }
                        };

            await Assert.ThrowsExceptionAsync<ArgumentException>(() => _target.HandleAsync(query));
        }

        [TestMethod]
        public async Task HandleAsync_EmptyCacheKey_ThrowException()
        {
            var query = new MockQuery
                        {
                            CacheQueryOptions = new CacheQueryOptions
                                                {
                                                    CacheKey = string.Empty
                                                }
                        };

            await Assert.ThrowsExceptionAsync<ArgumentException>(() => _target.HandleAsync(query));
        }

        [TestMethod]
        public async Task HandleAsync_HasAbsoluteExpiration_UseCacheWithAbsoluteExpiration()
        {
            var absoluteExpiration = DateTimeOffset.Now.AddMinutes(20);
            var key = "test";
            var query = new MockQuery
                        {
                            CacheQueryOptions = new CacheQueryOptions
                                                {
                                                    CacheKey = key,
                                                    AbsoluteExpiration = absoluteExpiration
                                                }
                        };
            var result = new MockResult();

            _queryHandler.Setup(x => x.HandleAsync(query)).Returns(Task.FromResult(result));

            await _target.HandleAsync(query);

            Assert.IsTrue(_cache.TheCache.ContainsKey(key));
            Assert.AreEqual(_cache.TheCache[key].options.AbsoluteExpiration, absoluteExpiration);
        }

        [TestMethod]
        public async Task HandleAsync_HasAbsoluteExpiration_UseCacheWithSliding()
        {
            var absoluteExpiration = DateTimeOffset.Now.AddMinutes(20);
            var key = "test";
            var slidingExpiration = TimeSpan.FromMinutes(20);
            var query = new MockQuery
                        {
                            CacheQueryOptions = new CacheQueryOptions
                                                {
                                                    CacheKey = key,
                                                    SlidingExpiration = slidingExpiration
                                                }
                        };
            var result = new MockResult();
            _queryHandler.Setup(x => x.HandleAsync(query)).Returns(Task.FromResult(result));

            await _target.HandleAsync(query);

            Assert.IsTrue(_cache.TheCache.ContainsKey(key));
            Assert.AreEqual(_cache.TheCache[key].options.SlidingExpiration, slidingExpiration);
        }
    }
}
