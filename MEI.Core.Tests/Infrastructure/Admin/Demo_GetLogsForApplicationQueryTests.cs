using System.Linq;
using System.Threading.Tasks;

using MEI.Core.DomainModels.Common;
using MEI.Core.Infrastructure.Admin.Queries;
using MEI.Core.Infrastructure.Data;
using MEI.Core.Infrastructure.Queries;
using MEI.Core.Infrastructure.Services;
using MEI.Core.Tests.Infrastructure.Helpers;
using MEI.Logging;

using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

namespace MEI.Core.Tests.Infrastructure.Admin
{
    [TestClass]
    public class Demo_GetLogsForApplicationQueryTests
    {
        private Mock<IUserResolverService> _userResolverService;
        private Mock<ICorrelationProvider> _correlationProvider;

        [TestInitialize]
        public void Initialize()
        {
            _userResolverService = new Mock<IUserResolverService>();
            _correlationProvider = new Mock<ICorrelationProvider>();
        }

        [TestMethod]
        public async Task HandleAsync_IsFilteredByApplicationName()
        {
            var appName = "App2";
            var environment = "Production";
            var size = 50;
            var query = new Demo_GetLogsForApplicationQuery
                        {
                            ApplicationName = appName,
                            Environment = environment,
                            Paging = new PageInfo
                                     {
                                         PageIndex = 0,
                                         PageSize = size
                                     }
                        };
            var options = new DbContextOptionsBuilder<CoreContext>()
                .UseInMemoryDatabase(nameof(Demo_GetLogsForApplicationQueryTests))
                .EnableSensitiveDataLogging()
                .Options;

            using var db = new CoreContext(options, _userResolverService.Object, _correlationProvider.Object);
            db.AddLogs(50, appName, environment);

            var target = new Demo_GetLogsForApplicationQueryHandler(db);

            Paged<Log> actual = await target.HandleAsync(query);

            Assert.IsTrue(actual.Items.Length > 0);
            Assert.AreEqual(actual.Items.Length, actual.Items.Where(x => x.AppName == appName).ToList().Count);
        }
    }
}
