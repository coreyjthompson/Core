using System;
using System.Data.Common;
using System.Threading.Tasks;

using MEI.Core.Commands;
using MEI.Core.Commands.Decorators;
using MEI.Core.Tests.Infrastructure.Mocks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

namespace MEI.Core.Tests.Infrastructure.Commands.Decorators
{
    [TestClass]
    public class DeadlockRetryCommandHandlerDecoratorTests
    {
        private DeadlockRetryCommandHandlerDecorator<MockCommand, MockResult> _target;
        private Mock<ICommandHandler<MockCommand, MockResult>> _commandHandler;

        [TestInitialize]
        public void Initialize()
        {
            _commandHandler = new Mock<ICommandHandler<MockCommand, MockResult>>();

            _target = new DeadlockRetryCommandHandlerDecorator<MockCommand, MockResult>(_commandHandler.Object);
        }

        [TestMethod]
        public async Task HandleAsync_DecoratedHandlerIsCalled()
        {
            var command = new MockCommand();

            await _target.HandleAsync(command);

            _commandHandler.Verify(x => x.HandleAsync(command));
        }

        [TestMethod]
        public async Task HandleAsync_ThrowsNonDeadlockException_RethrowIt()
        {
            var command = new MockCommand();
            _commandHandler.Setup(x => x.HandleAsync(command)).Throws<DivideByZeroException>();

            await Assert.ThrowsExceptionAsync<DivideByZeroException>(() => _target.HandleAsync(command));
        }

        [TestMethod]
        public async Task HandleAsync_ExceedsRetryLimit_RethrowException()
        {
            var command = new MockCommand();
            _commandHandler.SetupSequence(x => x.HandleAsync(command))
                .Throws<MockDeadlockException>()
                .Throws<MockDeadlockException>()
                .Throws<MockDeadlockException>()
                .Throws<MockDeadlockException>()
                .Throws<MockDeadlockException>()
                .Throws<DivideByZeroException>();

            await Assert.ThrowsExceptionAsync<DivideByZeroException>(() => _target.HandleAsync(command));
        }

        [TestMethod]
        public async Task HandleAsync_HasDeadlockException_DecoratedHandlerReturns()
        {
            var command = new MockCommand();
            var result = new MockResult();
            _commandHandler.SetupSequence(x => x.HandleAsync(command))
                .Throws<MockDeadlockException>()
                .Returns(Task.FromResult(result));

            var actual = await _target.HandleAsync(command);

            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public async Task HandleAsync_HasInnerDeadlockException_DecoratedHandlerReturns()
        {
            var command = new MockCommand();
            var result = new MockResult();
            _commandHandler.SetupSequence(x => x.HandleAsync(command))
                .Throws(new DivideByZeroException("test", new MockDeadlockException()))
                .Returns(Task.FromResult(result));

            var actual = await _target.HandleAsync(command);

            Assert.IsNotNull(actual);
        }
    }

    public class MockDeadlockException
        : DbException
    {
        public MockDeadlockException()
            : base("deadlock")
        {}
    }
}
