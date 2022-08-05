using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MEI.Core.DomainModels.Common;
using MEI.Core.Infrastructure.Data;
using MEI.Core.Infrastructure.Data.Helpers;
using MEI.Core.Infrastructure.Queries;

using Microsoft.EntityFrameworkCore;

namespace MEI.Core.Infrastructure.Clients.Queries
{
    public class GetAllClientsQuery
        : IQuery<List<Client>>
    {

    }

    public class GetAllClientsQueryHandler
        : IQueryHandler<GetAllClientsQuery, List<Client>>
    {
        private readonly CoreContext _db;

        public GetAllClientsQueryHandler(CoreContext db)
        {
            _db = db;
        }

        public async Task<List<Client>> HandleAsync(GetAllClientsQuery query)
        {
            return await _db.Clients.OrderBy(x => x.Id).ToListAsync();
        }

    }
}