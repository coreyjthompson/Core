using System;
using System.Threading.Tasks;

using MEI.Core.Commands;
using MEI.Core.Commands.Decorators;
using MEI.Core.Infrastructure;
using MEI.Core.Tests.Infrastructure.Mocks;

using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using NodaTime;

namespace MEI.Core.Tests.Infrastructure.Commands.Decorators
{
    [TestClass]
    public class RunTimeLogCommandDecoratorTests
    {
        private RunTimeLogCommandHandlerDecorator<MockCommand, MockResult> _target;
        private Mock<ICommandHandler<MockCommand, MockResult>> _commandHandler;
        private Mock<ILogger<RunTimeLogCommandHandlerDecorator<MockCommand, MockResult>>> _logger;
        private Mock<IStopwatch> _stopwatch;

        [TestInitialize]
        public void Initialize()
        {
            _commandHandler = new Mock<ICommandHandler<MockCommand, MockResult>>();
            _logger = new Mock<ILogger<RunTimeLogCommandHandlerDecorator<MockCommand, MockResult>>>();
            _stopwatch = new Mock<IStopwatch>();

            _target = new RunTimeLogCommandHandlerDecorator<MockCommand, MockResult>(_commandHandler.Object, _logger.Object, _stopwatch.Object);
        }

        [TestMethod]
        public async Task HandleAsync_TraceLogWithCommandInfoAndElapsedTime()
        {
            var command = new MockCommand();
            Duration elapsed = Duration.FromSeconds(10);
            _stopwatch.Setup(x => x.ElapsedDuration()).Returns(elapsed);

            await _target.HandleAsync(command);

            _logger.Verify(x => x.Log(LogLevel.Trace, It.IsAny<EventId>(), It.Is<It.IsAnyType>(y => y.ToString() == "MockCommand:[MockCommand]:0:00:00:10"), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()));
        }

        [TestMethod]
        public async Task HandleAsync_DecoratedHandlerIsCalled()
        {
            var command = new MockCommand();

            await _target.HandleAsync(command);

            _commandHandler.Verify(x => x.HandleAsync(command));
        }
    }
}
