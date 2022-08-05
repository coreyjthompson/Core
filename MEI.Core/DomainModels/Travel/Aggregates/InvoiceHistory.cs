using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using MEI.Core.DomainModels.Common;

namespace MEI.Core.DomainModels.Travel.Aggregates
{
    public class InvoiceHistoryLine
    {
        public int InvoiceId { get; set; }

        public string SabreInvoiceId { get; set; }

        public int? EventId { get; set; }

        public string EventName { get; set; }

        public int? ConsultantId { get; set; }

        public string ConsultantName { get; set; }

        public int? ClientId { get; set; }

        public long? Quantity { get; set; } = 1;

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Amount { get; set; }

        public int? AgencyServiceId { get; set; }

        public string AgencyServiceName { get; set; }

        public string ClientName { get; set; }

        

    }
}