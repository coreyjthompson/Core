using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MEI.Core.DomainModels.Common;
using MEI.Core.Infrastructure.Admin.Queries;
using MEI.Core.Infrastructure.Clients.Queries;
using MEI.Core.Infrastructure.Data;
using MEI.Core.Infrastructure.Queries;
using MEI.Core.Infrastructure.Services;
using MEI.Core.Tests.Infrastructure.Helpers;
using MEI.Logging;

using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

namespace MEI.Core.Tests.Infrastructure.Clients
{
    [TestClass]
    public class GetAllClientsQueryTests
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
            var query = new GetAllClientsQuery();
            var options = new DbContextOptionsBuilder<CoreContext>()
                .UseInMemoryDatabase(nameof(GetAllClientsQueryTests))
                .EnableSensitiveDataLogging()
                .Options;

            using var db = new CoreContext(options, _userResolverService.Object, _correlationProvider.Object);

            var target = new GetAllClientsQueryHandler(db);

            IList<Client> actual = await target.HandleAsync(query);

            Assert.IsTrue(actual.Count > 0);
        }
    }
}
