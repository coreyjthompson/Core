using System.Threading.Tasks;

using MEI.Core.DomainModels.Travel;
using MEI.Core.Infrastructure.Data;
using MEI.Core.Infrastructure.Queries;

using Microsoft.EntityFrameworkCore;

namespace MEI.Travel.Queries
{
    public class GetInvoiceByIdQuery
        : IQuery<Invoice>
    {
        public int Id { get; set; }

        public override string ToString()
        {
            return string.Format("[Id={0}]", Id);
        }
    }

    public class GetInvoiceByIdQueryHandler
        : IQueryHandler<GetInvoiceByIdQuery, Invoice>
    {
        private readonly CoreContext _db;

        public GetInvoiceByIdQueryHandler(CoreContext db)
        {
            _db = db;
        }

        public Task<Invoice> HandleAsync(GetInvoiceByIdQuery query)
        {
            return _db.TravelInvoices
                .Include("Client")
                .Include("LineItems")
                .Include("WorkflowSteps")
                .FirstOrDefaultAsync(x => x.Id == query.Id);
        }
    }
}