#region Using Statements

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using MEI.AbbVie.Infrastructure.Queries.Program;
using MEI.AbbVie.Infrastructure.Queries.Speaker;
using MEI.Core.Commands;
using MEI.Core.DomainModels.Common;
using MEI.Core.DomainModels.Travel;
using MEI.Core.Infrastructure.Ldap.Queries;
using MEI.Core.Queries;
using MEI.Travel.Commands;
using MEI.Travel.Queries;
using MEI.Web.Areas.Travel.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

#endregion

namespace MEI.Web.Areas.Travel.Pages.Invoices
{
    public class EditModel : PageModel
    {
        private const decimal defaultAmount = 25;
        private readonly IQueryProcessor _abbvieQueries;
        private readonly ICommandProcessor _commands;
        private readonly IQueryProcessor _queries;

        public EditModel(IQueryProcessor queries, ICommandProcessor commands, IQueryProcessor abbvieQueries)
        {
            _queries = queries;
            _commands = commands;
            _abbvieQueries = abbvieQueries;
        }


        [BindProperty] public InvoiceFormViewModel TravelInvoice { get; set; } = new InvoiceFormViewModel();

        public string LineItemsTotal { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var query = new GetInvoiceByIdQuery {Id = id};
            var invoice = await _queries.Execute(query);

            if (invoice != null)
            {
                TravelInvoice = new InvoiceFormViewModel
                {
                    PageAction = "edit",
                    SubmittingUser = User.Identity.Name,
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
            }
            else
            {
                return NotFound();
            }

            // if this invoice has already been submitted, then we redirect to view page
            if (TravelInvoice.WorkflowSteps.Any(w => w.WorkflowStepId == (int) WorkflowStepEnum.InvoiceSubmittedForPayment))
            {
                return RedirectToPage("/Invoices/View", new { Area = "Travel", id = id });
            }

            return Page();
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

        private Task<ActiveDirectoryUser> GetUser()
        {
            var userId = User.Identity.Name;

            var query = new FindByIdentityQuery {Username = userId};

            return _queries.Execute(query);
        }

        public IList<SelectListItem> GetTravelServices()
        {
            var query = new GetAllTravelServicesQuery();
            var types = _queries.Execute(query);

            return types.Result.Select(i => new SelectListItem {Value = i.Id.ToString(), Text = i.Name}).ToList();
        }

        private List<Tuple<string, ModelErrorCollection>> GetModelStateErrors()
        {
            if (!ModelState.IsValid)
            {
                return ModelState.Where(ms => ms.Value.Errors.Any()).Select(x => new Tuple<string, ModelErrorCollection>(x.Key, x.Value.Errors)).ToList();
            }

            return new List<Tuple<string, ModelErrorCollection>>();
        }

        private IList<Tuple<int, string, ModelErrorCollection>> GetLineItemErrors(List<Tuple<string, ModelErrorCollection>> errors)
        {
            var filtered = errors.Where(e => e.Item1.Contains("TravelInvoice.LineItems"));
            var fields = new List<Tuple<int, string, ModelErrorCollection>>();

            foreach (var error in filtered)
            {
                // Strip the index from the key and then try to parse it
                var clean = Regex.Match(error.Item1, @"\[([^)]*)\]").Groups[1].Value;

                if (int.TryParse(clean, out var index))
                {
                    // Split the key at the periods
                    var namespaces = error.Item1.Split(".");
                    // Get the last index in the array
                    var name = namespaces[^1];

                    // Add it to the new list
                    fields.Add(new Tuple<int, string, ModelErrorCollection>(index, name, error.Item2));
                }
            }

            return fields;
        }

        private void SetLineItemErrors(IList<Tuple<int, string, ModelErrorCollection>> errors)
        {
            foreach (var item in TravelInvoice.LineItems)
            {
                foreach (var error in errors)
                {
                    switch (error.Item2)
                    {
                        case "Amount":
                            item.LineItemError.AmountValidationMessage = error.Item3.FirstOrDefault()?.ErrorMessage;
                            item.LineItemError.IsAmountInvalid = true;

                            break;
                        case "ExpenseTypeId":
                            item.LineItemError.TravelServiceValidationMessage = error.Item3.FirstOrDefault()?.ErrorMessage;
                            item.LineItemError.IsTravelServiceInvalid = true;

                            break;
                    }
                }
            }
        }

        private void FillInvoiceSelections()
        {
            TravelInvoice.Consultants = new List<SelectListItem>();

            // Get the programs
            var eventQuery = new GetAllProgramsQuery();
            var events = _abbvieQueries.Execute(eventQuery).Result;

            TravelInvoice.Events = events.Select(p => new SelectListItem {Value = p.Id.ToString(), Text = p.Name}).ToList();
            //TravelInvoice.Events = _abbvieQueries.Execute(eventQuery).Result;

            // Get the speakers
            GetConsultantsForProgram(TravelInvoice.EventName);

            var total = TravelInvoice.LineItems.Sum(e => e.Amount);
            LineItemsTotal = total.ToString("C2");

            TravelInvoice.TravelServices = GetTravelServices();
        }

        private void GetConsultantsForProgram(string programName)
        {
            if (!string.IsNullOrEmpty(programName))
            {
                // Get the speakers
                var consultantQuery = new GetAllSpeakersForProgramQuery {ProgramName = programName};

                var data = _abbvieQueries.Execute(consultantQuery).Result;
                var consultants = data.Select(p => new SelectListItem {Value = p.ConsultantId.ToString(), Text = p.Consultant.GetFullName()}).ToList();

                TravelInvoice.Consultants = consultants;
            }
        }
    }
}