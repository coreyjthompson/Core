using System;
using System.Threading.Tasks;

using MEI.Core.Infrastructure;
using MEI.Core.Infrastructure.Queries;
using MEI.Core.Infrastructure.Queries.Decorators;
using MEI.Core.Tests.Infrastructure.Mocks;

using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using NodaTime;

namespace MEI.Core.Tests.Infrastructure.Queries.Decorators
{
    [TestClass]
    public class RunTimeLogQueryHandlerDecoratorTests
    {
        private RunTimeLogQueryHandlerDecorator<MockQuery, MockResult> _target;
        private Mock<IQueryHandler<MockQuery, MockResult>> _queryHandler;
        private Mock<ILogger<RunTimeLogQueryHandlerDecorator<MockQuery, MockResult>>> _logger;
        private Mock<IStopwatch> _stopwatch;

        [TestInitialize]
        public void Initialize()
        {
            _queryHandler = new Mock<IQueryHandler<MockQuery, MockResult>>();
            _logger = new Mock<ILogger<RunTimeLogQueryHandlerDecorator<MockQuery, MockResult>>>();
            _stopwatch = new Mock<IStopwatch>();

            _target = new RunTimeLogQueryHandlerDecorator<MockQuery, MockResult>(_queryHandler.Object, _logger.Object, _stopwatch.Object);
        }

        [TestMethod]
        public async Task HandleAsync_TraceLogWithQueryInfoAndElapsedTime()
        {
            var query = new MockQuery();
            Duration elapsed = Duration.FromSeconds(10);
            _stopwatch.Setup(x => x.ElapsedDuration()).Returns(elapsed);

            await _target.HandleAsync(query);

            _logger.Verify(x => x.Log(LogLevel.Trace, It.IsAny<EventId>(), It.Is<It.IsAnyType>(y => y.ToString() == "MockQuery:[MockQuery]:0:00:00:10"), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()));
        }

        [TestMethod]
        public async Task HandleAsync_DecoratedHandlerIsCalled()
        {
            var query = new MockQuery();

            await _target.HandleAsync(query);

            _queryHandler.Verify(x => x.HandleAsync(query));
        }
    }
}
