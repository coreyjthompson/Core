using System;
using System.Threading.Tasks;

using MEI.Core.Commands;
using MEI.Core.Tests.Infrastructure.Mocks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

namespace MEI.Core.Tests.Infrastructure.Commands
{
    [TestClass]
    public class DynamicCommandProcessorTests
    {
        private DynamicCommandProcessor _target;
        private Mock<IServiceProvider> _serviceProvider;
        private Mock<ICommandHandler<MockCommand, MockResult>> _commandHandler;

        [TestInitialize]
        public void Initialize()
        {
            _serviceProvider = new Mock<IServiceProvider>();
            _commandHandler = new Mock<ICommandHandler<MockCommand, MockResult>>();
            _serviceProvider.Setup(x => x.GetService(It.IsAny<Type>())).Returns(_commandHandler.Object);

            _target = new DynamicCommandProcessor(_serviceProvider.Object);
        }

        [TestMethod]
        public async Task Execute_BlueSky()
        {
            var command = new MockCommand();
            _commandHandler.Setup(x => x.HandleAsync(command)).Returns(Task.FromResult(new MockResult()));

            var actual = await _target.Execute(command);

            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public async Task Execute_CommandIsNull_ThrowException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => _target.Execute<ICommand<MockResult>>(null));
        }
    }
}
