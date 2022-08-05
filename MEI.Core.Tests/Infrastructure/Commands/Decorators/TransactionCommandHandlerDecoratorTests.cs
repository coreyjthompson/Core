using System.Threading.Tasks;
using System.Transactions;

using MEI.Core.Commands;
using MEI.Core.Commands.Decorators;
using MEI.Core.Tests.Infrastructure.Mocks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

namespace MEI.Core.Tests.Infrastructure.Commands.Decorators
{
    [TestClass]
    public class TransactionCommandHandlerDecoratorTests
    {
        private TransactionCommandHandlerDecorator<MockCommand, MockResult> _target;
        private Mock<ICommandHandler<MockCommand, MockResult>> _commandHandler;

        [TestInitialize]
        public void Initialize()
        {
            _commandHandler = new Mock<ICommandHandler<MockCommand, MockResult>>();

            _target = new TransactionCommandHandlerDecorator<MockCommand, MockResult>(_commandHandler.Object);
        }

        [TestMethod]
        public async Task HandleAsync_TransactionIsCompleted()
        {
            Transaction transaction = null;
            bool transactionCommitted = false;
            var command = new MockCommand();
            _commandHandler.Setup(x => x.HandleAsync(command)).Callback(() =>
            {
                transaction = Transaction.Current;
                transaction.TransactionCompleted += (sender, args) =>
                    transactionCommitted = args.Transaction.TransactionInformation.Status == TransactionStatus.Committed;
            });

            await _target.HandleAsync(command);

            Assert.IsNotNull(transaction);
            Assert.IsTrue(transactionCommitted);
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
