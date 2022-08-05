using System;
using System.Linq;
using System.Threading.Tasks;

using MEI.Core.Commands;
using MEI.Core.Infrastructure.Commands.Decorators;
using MEI.Core.Infrastructure.Data;
using MEI.Core.Infrastructure.Services;
using MEI.Core.Tests.Infrastructure.Mocks;
using MEI.Logging;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using NodaTime;

namespace MEI.Core.Tests.Infrastructure.Commands.Decorators
{
    [TestClass]
    public class AuditingCommandHandlerDecoratorTests
    {
        private Mock<IUserResolverService> _userResolverService;
        private Mock<IClock> _clock;
        private Mock<IConfiguration> _config;
        private Mock<ICorrelationProvider> _correlationProvider;
        private Mock<ICommandHandler<MockCommand, MockResult>> _commandHandler;

        [TestInitialize]
        public void Initialize()
        {
            _userResolverService = new Mock<IUserResolverService>();
            _clock = new Mock<IClock>();
            _config = new Mock<IConfiguration>();
            _correlationProvider = new Mock<ICorrelationProvider>();
            _commandHandler = new Mock<ICommandHandler<MockCommand, MockResult>>();
        }

        [TestMethod]
        public async Task HandleAsync_BlueSky()
        {
            var command = new MockCommand();

            var options = new DbContextOptionsBuilder<CoreContext>()
                .UseInMemoryDatabase(nameof(AuditingCommandHandlerDecoratorTests.HandleAsync_BlueSky)).Options;

            using (var db = new CoreContext(options, _userResolverService.Object, _correlationProvider.Object))
            {
                var target = new AuditingCommandHandlerDecorator<MockCommand, MockResult>(_commandHandler.Object,
                    _userResolverService.Object,
                    db,
                    _clock.Object,
                    _config.Object);

                await target.HandleAsync(command);

                var items = db.AuditEntries.ToList();

                Assert.AreEqual(1, items.Count);
            }
        }

        [TestMethod]
        public async Task HandleAsync_HasEnvironmentInApplicationOptions_UseIt()
        {
            var environment = "AnEnvironment";
            _config.Setup(x => x["ApplicationOptions:Environment"]).Returns(environment);
            var command = new MockCommand();

            var options = new DbContextOptionsBuilder<CoreContext>()
                .UseInMemoryDatabase(nameof(AuditingCommandHandlerDecoratorTests.HandleAsync_HasEnvironmentInApplicationOptions_UseIt)).Options;

            using (var db = new CoreContext(options, _userResolverService.Object, _correlationProvider.Object))
            {
                var target = new AuditingCommandHandlerDecorator<MockCommand, MockResult>(_commandHandler.Object,
                    _userResolverService.Object,
                    db,
                    _clock.Object,
                    _config.Object);

                await target.HandleAsync(command);

                var items = db.AuditEntries.ToList();

                Assert.AreEqual(1, items.Count);
                Assert.AreEqual(environment, items[0].Environment);
            }
        }

        [TestMethod]
        public async Task HandleAsync_HasEnvironmentInRoot_UseIt()
        {
            var environment = "AnEnvironment";
            _config.Setup(x => x["Environment"]).Returns(environment);
            var command = new MockCommand();

            var options = new DbContextOptionsBuilder<CoreContext>()
                .UseInMemoryDatabase(nameof(AuditingCommandHandlerDecoratorTests.HandleAsync_HasEnvironmentInRoot_UseIt)).Options;

            using (var db = new CoreContext(options, _userResolverService.Object, _correlationProvider.Object))
            {
                var target = new AuditingCommandHandlerDecorator<MockCommand, MockResult>(_commandHandler.Object,
                    _userResolverService.Object,
                    db,
                    _clock.Object,
                    _config.Object);

                await target.HandleAsync(command);

                var items = db.AuditEntries.ToList();

                Assert.AreEqual(1, items.Count);
                Assert.AreEqual(environment, items[0].Environment);
            }
        }

        [TestMethod]
        public async Task HandleAsync_HasAppNameInApplicationOptions_UseIt()
        {
            var appName = "AnAppName";
            _config.Setup(x => x["ApplicationOptions:AppName"]).Returns(appName);
            var command = new MockCommand();

            var options = new DbContextOptionsBuilder<CoreContext>()
                .UseInMemoryDatabase(nameof(AuditingCommandHandlerDecoratorTests.HandleAsync_HasAppNameInApplicationOptions_UseIt)).Options;

            using (var db = new CoreContext(options, _userResolverService.Object, _correlationProvider.Object))
            {
                var target = new AuditingCommandHandlerDecorator<MockCommand, MockResult>(_commandHandler.Object,
                    _userResolverService.Object,
                    db,
                    _clock.Object,
                    _config.Object);

                await target.HandleAsync(command);

                var items = db.AuditEntries.ToList();

                Assert.AreEqual(1, items.Count);
                Assert.AreEqual(appName, items[0].AppName);
            }
        }

        [TestMethod]
        public async Task HandleAsync_HasAppNameInRoot_UseIt()
        {
            var appName = "AnAppName";
            _config.Setup(x => x["AppName"]).Returns(appName);
            var command = new MockCommand();

            var options = new DbContextOptionsBuilder<CoreContext>()
                .UseInMemoryDatabase(nameof(AuditingCommandHandlerDecoratorTests.HandleAsync_HasAppNameInRoot_UseIt)).Options;

            using (var db = new CoreContext(options, _userResolverService.Object, _correlationProvider.Object))
            {
                var target = new AuditingCommandHandlerDecorator<MockCommand, MockResult>(_commandHandler.Object,
                    _userResolverService.Object,
                    db,
                    _clock.Object,
                    _config.Object);

                await target.HandleAsync(command);

                var items = db.AuditEntries.ToList();

                Assert.AreEqual(1, items.Count);
                Assert.AreEqual(appName, items[0].AppName);
            }
        }

        [TestMethod]
        public async Task HandleAsync_MakeSureHasProperWhenExecuted()
        {
            var command = new MockCommand();
            var whenExecuted = Instant.FromDateTimeOffset(new DateTimeOffset(2019, 12, 25, 8, 0, 0, TimeSpan.FromHours(-6)));
            _clock.Setup(x => x.GetCurrentInstant()).Returns(whenExecuted);

            var options = new DbContextOptionsBuilder<CoreContext>()
                .UseInMemoryDatabase(nameof(AuditingCommandHandlerDecoratorTests.HandleAsync_MakeSureHasProperWhenExecuted)).Options;

            using (var db = new CoreContext(options, _userResolverService.Object, _correlationProvider.Object))
            {
                var target = new AuditingCommandHandlerDecorator<MockCommand, MockResult>(_commandHandler.Object,
                    _userResolverService.Object,
                    db,
                    _clock.Object,
                    _config.Object);

                await target.HandleAsync(command);

                var items = db.AuditEntries.ToList();

                Assert.AreEqual(items[0].WhenExecuted, whenExecuted.ToDateTimeOffset());
            }
        }

        [TestMethod]
        public async Task HandleAsync_MakeSureHasProperUserName()
        {
            var command = new MockCommand();
            var userName = "someusername";
            _userResolverService.Setup(x => x.GetUserName()).Returns(userName);

            var options = new DbContextOptionsBuilder<CoreContext>()
                .UseInMemoryDatabase(nameof(AuditingCommandHandlerDecoratorTests.HandleAsync_MakeSureHasProperUserName)).Options;

            using (var db = new CoreContext(options, _userResolverService.Object, _correlationProvider.Object))
            {
                var target = new AuditingCommandHandlerDecorator<MockCommand, MockResult>(_commandHandler.Object,
                    _userResolverService.Object,
                    db,
                    _clock.Object,
                    _config.Object);

                await target.HandleAsync(command);

                var items = db.AuditEntries.ToList();

                Assert.AreEqual(items[0].UserName, userName);
            }
        }
    }
}
