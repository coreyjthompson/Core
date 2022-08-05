using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MEI.Core.DomainModels.Travel;
using MEI.Core.Infrastructure.Data;
using MEI.Core.Infrastructure.Queries;

using Microsoft.EntityFrameworkCore;

namespace MEI.Travel.Queries
{
    public class GetAllTravelServicesQuery
        : IQuery<List<AgencyService>>
    {
    }

    public class GetAllTravelServicesQueryHandler
        : IQueryHandler<GetAllTravelServicesQuery, List<AgencyService>>
    {
        private readonly CoreContext _db;

        public GetAllTravelServicesQueryHandler(CoreContext db)
        {
            _db = db;
        }

        public async Task<List<AgencyService>> HandleAsync(GetAllTravelServicesQuery query)
        {
            return await _db.AgencyServices.OrderBy(x => x.Name).ToListAsync();
        }
    }
}