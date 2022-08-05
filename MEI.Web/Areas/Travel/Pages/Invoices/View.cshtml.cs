#region Using Statements

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MEI.Core.Commands;
using MEI.Core.DomainModels.Travel;
using MEI.Core.Queries;
using MEI.Travel.Queries;
using MEI.Web.Areas.Travel.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

#endregion

namespace MEI.Web.Areas.Travel.Pages.Invoices
{
    public class ViewModel : PageModel
    {
        private const decimal defaultAmount = 25;
        private readonly ICommandProcessor _commands;
        private readonly IQueryProcessor _queries;

        public ViewModel(IQueryProcessor queries, ICommandProcessor commands)
        {
            _queries = queries;
            _commands = commands;
        }

        [BindProperty] public InvoiceFormViewModel TravelInvoice { get; set; } = new InvoiceFormViewModel();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var query = new GetInvoiceByIdQuery {Id = id};
            // Get the invoice from repo
            var invoice = await _queries.Execute(query);

            if (invoice != null)
            {
                TravelInvoice = new InvoiceFormViewModel
                {
                    InvoiceId = invoice.Id,
                    ClientName = invoice.Client.Name,
                    EventId = invoice.EventId,
                    EventName = invoice.EventName,
                    ConsultantId = invoice.ConsultantId,
                    ConsultantName = invoice.ConsultantName,
                    SabreInvoiceId = invoice.SabreInvoiceId,
                    LineItems = GetLineItems(invoice.LineItems),
                    WorkflowSteps = invoice.WorkflowSteps
                };
                SetClientExpenseCategory();

                // Total up the lines
                var total = TravelInvoice.LineItems.Sum(e => e.Amount);
                TravelInvoice.TotalAmount = total.ToString("C2");

                // Get the Expense type's name
                var types = GetTravelServices();

                foreach (var item in TravelInvoice.LineItems)
                {
                    item.TravelServiceName = string.Format("Agency Service Fee for {0}", types.FirstOrDefault(t => t.Id == item.TravelServiceId)?.Name);
                }

                return Page();
            }

            return NotFound();
        }

        public IList<AgencyService> GetTravelServices()
        {
            var query = new GetAllTravelServicesQuery();
            var types = _queries.Execute(query);

            return types.Result.ToList();
        }

        private IList<InvoiceFormViewModel.LineItem> GetLineItems(IList<InvoiceLineItem> items)
        {
            var list = items.Select(
                i => new InvoiceFormViewModel.LineItem
                {
                    Id = i.Id,
                    TravelServiceId = i.AgencyServiceId,
                    Quantity = i.Quantity,
                    Amount = i.Amount,
                    AmountAsString = i.Amount.ToString("C2")
                }
            ).ToList();

            return list;
        }

        private void SetClientExpenseCategory()
        {
            switch (TravelInvoice.ClientName.ToLower())
            {
                case "abbvie":
                    TravelInvoice.Description = "Travel Booking";
                    break;
                default:
                    TravelInvoice.Description = "Travel Booking Fees";
                    break;

            }

        }
    }
}