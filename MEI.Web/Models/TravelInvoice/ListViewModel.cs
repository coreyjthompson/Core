using System.Collections.Generic;

using MEI.Core.DomainModels.Travel;

namespace MEI.Web.Models.TravelInvoice
{
    public class ListViewModel
    {
        public IList<Invoice> Invoices { get; set; }
    }
}
