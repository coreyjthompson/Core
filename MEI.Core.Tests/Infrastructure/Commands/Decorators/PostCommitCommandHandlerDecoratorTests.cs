using System;
using System.Threading.Tasks;

using MEI.Core.Commands;
using MEI.Core.Commands.Decorators;
using MEI.Core.Tests.Infrastructure.Mocks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

namespace MEI.Core.Tests.Infrastructure.Commands.Decorators
{
    [TestClass]
    public class PostCommitCommandHandlerDecoratorTests
    {
        private PostCommitCommandHandlerDecorator<MockCommand, MockResult> _target;
        private Mock<ICommandHandler<MockCommand, MockResult>> _commandHandler;
        private Mock<IPostCommitRegistrator> _postCommitRegistrator;

        [TestInitialize]
        public void Initialize()
        {
            _commandHandler = new Mock<ICommandHandler<MockCommand, MockResult>>();
            _postCommitRegistrator = new Mock<IPostCommitRegistrator>();

            _target = new PostCommitCommandHandlerDecorator<MockCommand, MockResult>(_commandHandler.Object, _postCommitRegistrator.Object);
        }

        [TestMethod]
        public async Task HandleAsync_ExecutePostCommitAfterDecoratedHandler()
        {
            var callOrder = 0;
            var command = new MockCommand();
            _commandHandler.Setup(x => x.HandleAsync(command)).Callback(() => Assert.AreEqual(0, callOrder++));
            _postCommitRegistrator.Setup(x => x.ExecuteActions()).Callback(() => Assert.AreEqual(1, callOrder++));

            await _target.HandleAsync(command);
        }

        [TestMethod]
        public async Task HandleAsync_RegistratorIsReset()
        {
            var command = new MockCommand();

            await _target.HandleAsync(command);

            _postCommitRegistrator.Verify(x => x.Reset(), Times.Once);
        }

        [TestMethod]
        public async Task HandleAsync_RegistratorIsResetEvenIfException()
        {
            var command = new MockCommand();
            _commandHandler.Setup(x => x.HandleAsync(command)).Throws<DivideByZeroException>();

            try
            {
                await _target.HandleAsync(command);
            }
            catch (DivideByZeroException)
            {}

            _postCommitRegistrator.Verify(x => x.Reset(), Times.Once);
        }
    }
}
