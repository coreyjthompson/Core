using MEI.Core.DomainModels.Common;

namespace MEI.Core.DomainModels.Travel
{
    public class InvoiceWorkflowStatus
        : WorkflowStatus
    {
        public int InvoiceId { get; set; }

        public virtual Invoice Invoice { get; set; }
    }

}