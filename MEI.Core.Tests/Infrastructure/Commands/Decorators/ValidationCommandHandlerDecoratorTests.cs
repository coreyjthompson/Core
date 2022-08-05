using System;
using System.Threading.Tasks;

using MEI.Core.Commands;
using MEI.Core.Commands.Decorators;
using MEI.Core.Tests.Infrastructure.Mocks;
using MEI.Core.Validation;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

namespace MEI.Core.Tests.Infrastructure.Commands.Decorators
{
    [TestClass]
    public class ValidationCommandHandlerDecoratorTests
    {
        private ValidationCommandHandlerDecorator<MockCommand, MockResult> _target;
        private Mock<IValidator> _validator;
        private Mock<ICommandHandler<MockCommand, MockResult>> _commandHandler;

        [TestInitialize]
        public void Initialize()
        {
            _validator = new Mock<IValidator>();
            _commandHandler = new Mock<ICommandHandler<MockCommand, MockResult>>();

            _target = new ValidationCommandHandlerDecorator<MockCommand, MockResult>(_validator.Object, _commandHandler.Object);
        }

        [TestMethod]
        public async Task HandleAsync_CommandIsNull_ThrowException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => _target.HandleAsync(null));
        }

        [TestMethod]
        public async Task HandleAsync_ValidatorIsCalled()
        {
            var command = new MockCommand();

            await _target.HandleAsync(command);

            _validator.Verify(x => x.ValidateObject(command), Times.Once);
        }

        [TestMethod]
        public async Task HandleAsync_DecoratedHandleIsCalled()
        {
            var command = new MockCommand();

            await _target.HandleAsync(command);

            _commandHandler.Verify(x => x.HandleAsync(command));
        }
    }
}
