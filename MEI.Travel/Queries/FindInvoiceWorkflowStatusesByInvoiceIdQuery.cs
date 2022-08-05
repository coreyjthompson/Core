using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MEI.Core.DomainModels.Travel;
using MEI.Core.Infrastructure.Data;
using MEI.Core.Infrastructure.Queries;

using Microsoft.EntityFrameworkCore;

namespace MEI.Travel.Queries
{
    public class FindInvoiceWorkflowStatusesByInvoiceIdQuery
        : IQuery<List<InvoiceWorkflowStatus>>
    {
        public int InvoiceId { get; set; }

        public override string ToString()
        {
            return string.Format("[InvoiceId={0}]", InvoiceId);
        }
    }

    public class FindInvoiceWorkflowStatusesByInvoiceIdQueryHandler
        : IQueryHandler<FindInvoiceWorkflowStatusesByInvoiceIdQuery, List<InvoiceWorkflowStatus>>
    {
        private readonly CoreContext _db;

        public FindInvoiceWorkflowStatusesByInvoiceIdQueryHandler(CoreContext db)
        {
            _db = db;
        }

        public async Task<List<InvoiceWorkflowStatus>> HandleAsync(FindInvoiceWorkflowStatusesByInvoiceIdQuery query)
        {
            return await _db.TravelInvoiceWorkflowStatuses.Where(x => x.InvoiceId == query.InvoiceId).ToListAsync();
        }
    }
}