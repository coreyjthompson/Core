using System;
using System.Threading.Tasks;

using MEI.Core.Infrastructure.Queries;
using MEI.Core.Infrastructure.Queries.Decorators;
using MEI.Core.Tests.Infrastructure.Mocks;
using MEI.Core.Validation;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

namespace MEI.Core.Tests.Infrastructure.Queries.Decorators
{
    [TestClass]
    public class ValidationQueryHandlerDecoratorTests
    {
        private ValidationQueryHandlerDecorator<MockQuery, MockResult> _target;
        private Mock<IValidator> _validator;
        private Mock<IQueryHandler<MockQuery, MockResult>> _queryHandler;

        [TestInitialize]
        public void Initialize()
        {
            _validator = new Mock<IValidator>();
            _queryHandler = new Mock<IQueryHandler<MockQuery, MockResult>>();

            _target = new ValidationQueryHandlerDecorator<MockQuery, MockResult>(_validator.Object, _queryHandler.Object);
        }

        [TestMethod]
        public async Task HandleAsync_QueryIsNull_ThrowException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => _target.HandleAsync(null));
        }

        [TestMethod]
        public async Task HandleAsync_ValidatorIsCalled()
        {
            var query = new MockQuery();

            await _target.HandleAsync(query);

            _validator.Verify(x => x.ValidateObject(query), Times.Once);
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
