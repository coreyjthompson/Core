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
    public class GetInvoiceHistoryByStatusIdQuery
        : IQuery<IList<InvoiceHistoryLine>>
    {
        public int StatusId { get; set; }

        public override string ToString()
        {
            return string.Format("[StatusId={0}]", StatusId);
        }
    }

    public class GetInvoiceHistoryByStatusIdQueryHandler
        : IQueryHandler<GetInvoiceHistoryByStatusIdQuery, IList<InvoiceHistoryLine>>
    {
        private readonly CoreContext _db;

        public GetInvoiceHistoryByStatusIdQueryHandler(CoreContext db)
        {
            _db = db;
        }

        public async Task<IList<InvoiceHistoryLine>> HandleAsync(GetInvoiceHistoryByStatusIdQuery query)
        {
            var status = await _db.TravelInvoiceWorkflowStatuses.FirstOrDefaultAsync(s => s.Id == query.StatusId);
            if(status != null)
            {
                var date = status.WhenCreated.DateTime;
                var invoiceId = status.InvoiceId;
                var lines = await _db.InvoiceHistoryLines
                    .FromSql($"SELECT * FROM dbo.vw_InvoiceTemporalHistory FOR SYSTEM_TIME AS OF {{0}}", date.ToUniversalTime()).Where(x => x.InvoiceId == invoiceId)
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