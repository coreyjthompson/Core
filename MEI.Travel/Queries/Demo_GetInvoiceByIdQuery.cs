using System.Threading.Tasks;

using MEI.Core.DomainModels.Travel;
using MEI.Core.Infrastructure.Data;
using MEI.Core.Infrastructure.Queries;

using Microsoft.EntityFrameworkCore;

namespace MEI.Travel.Queries
{
    public class Demo_GetInvoiceByIdQuery
        : IQuery<Invoice>
    {
        public int Id { get; set; }

        public override string ToString()
        {
            return string.Format("[Id={0}]", Id);
        }
    }

    public class Demo_GetInvoiceByIdQueryHandler
        : IQueryHandler<Demo_GetInvoiceByIdQuery, Invoice>
    {
        private readonly CoreContext _db;

        public Demo_GetInvoiceByIdQueryHandler(CoreContext db)
        {
            _db = db;
        }

        public Task<Invoice> HandleAsync(Demo_GetInvoiceByIdQuery query)
        {
            return _db.TravelInvoices
                .FirstOrDefaultAsync(x => x.Id == query.Id);
        }
    }
}