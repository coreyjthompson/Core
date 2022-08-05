using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MEI.Core.DomainModels.Travel;
using MEI.Core.DomainModels.Travel.Aggregates;
using MEI.Core.Infrastructure.Data;
using MEI.Core.Infrastructure.Data.Helpers;
using MEI.Core.Infrastructure.Queries;

using Microsoft.EntityFrameworkCore;

namespace MEI.Travel.Queries
{
    public class GetInvoiceHistoryByInvoiceIdQuery
        : IQuery<IList<InvoiceHistoryLine>>
    {
        public int InvoiceId { get; set; }

        public override string ToString()
        {
            return string.Format("[InvoiceId={0}]", InvoiceId);
        }
    }

    public class GetInvoiceHistoryByInvoiceIdQueryHandler
        : IQueryHandler<GetInvoiceHistoryByInvoiceIdQuery, IList<InvoiceHistoryLine>>
    {
        private readonly CoreContext _db;

        public GetInvoiceHistoryByInvoiceIdQueryHandler(CoreContext db)
        {
            _db = db;
        }

        public async Task<IList<InvoiceHistoryLine>> HandleAsync(GetInvoiceHistoryByInvoiceIdQuery query)
        {
            var invoice = _db.TravelInvoices.FirstOrDefaultAsync(s => s.Id == query.InvoiceId);
            if(invoice != null)
            {
                var lines = await _db.InvoiceHistoryLines
                    .FromSql("SELECT * FROM dbo.vw_InvoiceTemporalHistory FOR SYSTEM_TIME").Where(x => x.InvoiceId == query.InvoiceId)
                    .ToListAsync();

                return lines.Select(l => new InvoiceHistoryLine {
                                        InvoiceId = l.InvoiceId,
                                        ClientName = l.ClientName,
                                        ConsultantId = l.ConsultantId,
                                        ConsultantName = l.ConsultantName,
                                        EventId = l.EventId,
                                        EventName = l.EventName,
                                        AgencyServiceId = l.AgencyServiceId,
                                        AgencyServiceName = l.AgencyServiceName,
                                        Amount = l.Amount,
                                        Quantity = l.Quantity
                                    }).ToList();
            }

            return new List<InvoiceHistoryLine>();
        }
    }
}