using System.Collections.Generic;

using MEI.Core.DomainModels.Common;

namespace MEI.Core.DomainModels.Travel
{
    public class Invoice
    {
        public int Id { get; set; }

        public string SabreInvoiceId { get; set; }

        public int? EventId { get; set; }

        public string EventName { get; set; }

        public int? ConsultantId { get; set; }

        public string ConsultantName { get; set; }

        public int? ClientId { get; set; }

        public virtual Client Client { get; set; }

        public virtual IList<InvoiceLineItem> LineItems { get; set; } = new List<InvoiceLineItem>();

        public virtual IList<InvoiceWorkflowStatus> WorkflowSteps { get; set; } = new List<InvoiceWorkflowStatus>();
    }
}