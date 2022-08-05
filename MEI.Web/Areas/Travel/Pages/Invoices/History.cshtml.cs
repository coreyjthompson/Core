using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MEI.Core.DomainModels.Travel.Aggregates;
using MEI.Core.Queries;
using MEI.Travel.Queries;
using MEI.Web.Areas.Travel.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MEI.Web.Areas.Travel.Pages.Invoices
{
    public class HistoryModel : PageModel
    {
        private readonly IQueryProcessor _queries;
        public HistoryModel(IQueryProcessor queries)
        {
            _queries = queries;
        }

        [BindProperty] 
        public InvoiceFormViewModel TravelInvoice { get; set; } = new InvoiceFormViewModel();

        public IList<LineItem> HistoryLines { get; set; }

        public string ClientName { get; set; }

        public string EventName { get; set; }

        public string ConsultantName { get; set; }

        public string SabreInvoiceId { get; set; }

        public int WorkItemId { get; set; }

        public void OnGet(int id)
        {
            WorkItemId = id;
        }

        public class LineItem
        {
            public int InvoiceId { get; set; }

            public int? ClientId { get; set; }

            public string ClientName { get; set; }

            public int? ConsultantId { get; set; }

            public string ConsultantName { get; set; }

            public int? EventId { get; set; }

            public string EventName { get; set; }

            public string SabreInvoiceId { get; set; }

            public int AgencyServiceId { get; set; }

            public string AgencyServiceName { get; set; }

            public decimal Amount { get; set; }
        }
    }
}
