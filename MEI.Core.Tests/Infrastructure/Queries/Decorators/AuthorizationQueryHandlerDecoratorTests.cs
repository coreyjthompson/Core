using System.Security;
using System.Security.Principal;
using System.Threading.Tasks;

using MEI.Core.Infrastructure.Queries;
using MEI.Core.Infrastructure.Queries.Decorators;
using MEI.Core.Tests.Infrastructure.Mocks;

using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

namespace MEI.Core.Tests.Infrastructure.Queries.Decorators
{
    [TestClass]
    public class AuthorizationQueryHandlerDecoratorTests
    {
        private AuthorizationQueryHandlerDecorator<MockQuery, MockResult> _target;
        private Mock<IQueryHandler<MockQuery, MockResult>> _queryHandler;
        private Mock<IPrincipal> _principal;
        private Mock<ILogger<AuthorizationQueryHandlerDecorator<MockQuery, MockResult>>> _logger;
        private Mock<IIdentity> _identity;

        [TestInitialize]
        public void Initialize()
        {
            _queryHandler = new Mock<IQueryHandler<MockQuery, MockResult>>();
            _principal = new Mock<IPrincipal>();
            _identity = new Mock<IIdentity>();
            _principal.SetupGet(x => x.Identity).Returns(_identity.Object);

            _logger = new Mock<ILogger<AuthorizationQueryHandlerDecorator<MockQuery, MockResult>>>();

            _target = new AuthorizationQueryHandlerDecorator<MockQuery, MockResult>(_queryHandler.Object, _principal.Object, _logger.Object);
        }

        [TestMethod]
        public async Task HandleAsync_NamespaceIsNotAdmin_DoNotThrowException()
        {
            var query = new MockQuery();

            await _target.HandleAsync(query);
        }

        [TestMethod]
        public async Task HandleAsync_UserIsInAdminRole_DoNotThrowException()
        {
            _principal.Setup(x => x.IsInRole("Admin")).Returns(true);

            var query = new MockQuery();

            await _target.HandleAsync(query);
        }

        [TestMethod]
        public async Task HandleAsync_NamespaceIsAdminAndUseIsNotInAdminRole_ThrowException()
        {
            var queryHandler = new Mock<IQueryHandler<MockAdminQuery, MockResult>>();
            var logger = new Mock<ILogger<AuthorizationQueryHandlerDecorator<MockAdminQuery, MockResult>>>();
            var target = new AuthorizationQueryHandlerDecorator<MockAdminQuery, MockResult>(queryHandler.Object, _principal.Object, logger.Object);
            var query = new MockAdminQuery();

            await Assert.ThrowsExceptionAsync<SecurityException>(() => target.HandleAsync(query));
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
