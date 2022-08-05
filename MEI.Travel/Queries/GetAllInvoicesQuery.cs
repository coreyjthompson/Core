using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MEI.Core.DomainModels.Travel;
using MEI.Core.Infrastructure.Data;
using MEI.Core.Infrastructure.Data.Helpers;
using MEI.Core.Infrastructure.Queries;

using Microsoft.EntityFrameworkCore;

namespace MEI.Travel.Queries
{
    public class GetAllInvoicesQuery
        : IQuery<IList<Invoice>>
    {
    }

    public class GetAllInvoicesQueryHandler
        : IQueryHandler<GetAllInvoicesQuery, IList<Invoice>>
    {
        private readonly CoreContext _db;

        public GetAllInvoicesQueryHandler(CoreContext db)
        {
            _db = db;
        }

        public async Task<IList<Invoice>> HandleAsync(GetAllInvoicesQuery query)
        {
            return await _db.TravelInvoices
                .Include("Client")
                .Include("LineItems")
                .Include("WorkflowSteps")
                .OrderBy(x => x.Id).ToListAsync();
        }
    }
}