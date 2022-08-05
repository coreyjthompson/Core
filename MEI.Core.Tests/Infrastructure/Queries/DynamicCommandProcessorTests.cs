using System;
using System.Threading.Tasks;

using MEI.Core.Infrastructure.Queries;
using MEI.Core.Queries;
using MEI.Core.Tests.Infrastructure.Mocks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

namespace MEI.Core.Tests.Infrastructure.Queries
{
    [TestClass]
    public class DynamicQueryProcessorTests
    {
        private DynamicQueryProcessor _target;
        private Mock<IServiceProvider> _serviceProvider;
        private Mock<IQueryHandler<MockQuery, MockResult>> _queryHandler;

        [TestInitialize]
        public void Initialize()
        {
            _serviceProvider = new Mock<IServiceProvider>();
            _queryHandler = new Mock<IQueryHandler<MockQuery, MockResult>>();
            _serviceProvider.Setup(x => x.GetService(It.IsAny<Type>())).Returns(_queryHandler.Object);

            _target = new DynamicQueryProcessor(_serviceProvider.Object);
        }

        [TestMethod]
        public async Task Execute_BlueSky()
        {
            var query = new MockQuery();
            _queryHandler.Setup(x => x.HandleAsync(query)).Returns(Task.FromResult(new MockResult()));

            var actual = await _target.Execute(query);

            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public async Task Execute_QueryIsNull_ThrowException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => _target.Execute<IQuery<MockResult>>(null));
        }
    }
}
