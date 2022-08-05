using System.Security;
using System.Security.Principal;
using System.Threading.Tasks;

using MEI.Core.Commands;
using MEI.Core.Commands.Decorators;
using MEI.Core.Tests.Infrastructure.Mocks;

using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

namespace MEI.Core.Tests.Infrastructure.Commands.Decorators
{
    [TestClass]
    public class AuthorizationCommandHandlerDecoratorTests
    {
        private AuthorizationCommandHandlerDecorator<MockCommand, MockResult> _target;
        private Mock<ICommandHandler<MockCommand, MockResult>> _commandHandler;
        private Mock<IPrincipal> _principal;
        private Mock<ILogger<AuthorizationCommandHandlerDecorator<MockCommand, MockResult>>> _logger;
        private Mock<IIdentity> _identity;

        [TestInitialize]
        public void Initialize()
        {
            _commandHandler = new Mock<ICommandHandler<MockCommand, MockResult>>();
            _principal = new Mock<IPrincipal>();
            _identity = new Mock<IIdentity>();
            _principal.SetupGet(x => x.Identity).Returns(_identity.Object);

            _logger = new Mock<ILogger<AuthorizationCommandHandlerDecorator<MockCommand, MockResult>>>();

            _target = new AuthorizationCommandHandlerDecorator<MockCommand, MockResult>(_commandHandler.Object, _principal.Object, _logger.Object);
        }

        [TestMethod]
        public async Task HandleAsync_NamespaceIsNotAdmin_DoNotThrowException()
        {
            var command = new MockCommand();

            await _target.HandleAsync(command);
        }

        [TestMethod]
        public async Task HandleAsync_UserIsInAdminRole_DoNotThrowException()
        {
            _principal.Setup(x => x.IsInRole("Admin")).Returns(true);

            var command = new MockCommand();

            await _target.HandleAsync(command);
        }

        [TestMethod]
        public async Task HandleAsync_NamespaceIsAdminAndUseIsNotInAdminRole_ThrowException()
        {
            var commandHandler = new Mock<ICommandHandler<MockAdminCommand, MockResult>>();
            var logger = new Mock<ILogger<AuthorizationCommandHandlerDecorator<MockAdminCommand, MockResult>>>();
            var target = new AuthorizationCommandHandlerDecorator<MockAdminCommand, MockResult>(commandHandler.Object, _principal.Object, logger.Object);
            var command = new MockAdminCommand();

            await Assert.ThrowsExceptionAsync<SecurityException>(() => target.HandleAsync(command));
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
