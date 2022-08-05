#region Using Statements

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
    public class CreateModel : PageModel
    {
        private const decimal defaultAmount = 25;
        private readonly IQueryProcessor _abbvieQueries;
        private readonly ICommandProcessor _commands;
        private readonly IQueryProcessor _queries;

        public CreateModel(IQueryProcessor queries, ICommandProcessor commands, IQueryProcessor abbvieQueries)
        {
            _queries = queries;
            _commands = commands;
            _abbvieQueries = abbvieQueries;
        }

        [BindProperty] public InvoiceFormViewModel TravelInvoice { get; set; } = new InvoiceFormViewModel();

        public string LineItemsTotal { get; set; }

        public IActionResult OnGet()
        {
            TravelInvoice.PageAction = "add";
            TravelInvoice.SubmittingUser = User.Identity.Name;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var workflowSteps = new List<WorkflowStepEnum>();

            if (TravelInvoice.FormAction == "cancel")
            {
                return RedirectToPage("./Index");
            }

            if (TravelInvoice.FormAction == "save")
            {
                workflowSteps.Add(WorkflowStepEnum.InvoiceCreated);
            }

            if (TravelInvoice.FormAction == "submit")
            {
                workflowSteps.Add(WorkflowStepEnum.InvoiceCreated);
                workflowSteps.Add(WorkflowStepEnum.InvoiceSubmittedForPayment);
            }

            // Validate for submission
            if (!ModelState.IsValid)
            {
                // Get any errors in the line items
                var errors = GetLineItemErrors(GetModelStateErrors());
                // Set the validation errors on each line for each error
                SetLineItemErrors(errors);

                //FillInvoiceSelections();
                return Page();
            }

            // Move viewmodels into models and save
            var lineItems = TravelInvoice.LineItems.Select(l => new InvoiceLineItem {Id = l.Id, AgencyServiceId = l.TravelServiceId, Quantity = l.Quantity, Amount = decimal.Parse(l.AmountAsString, NumberStyles.Any)}).ToList();

            await _commands.Execute(
                new AddInvoiceCommand
                {
                    ClientName = TravelInvoice.ClientName,
                    EventId = TravelInvoice.EventId,
                    ConsultantId = TravelInvoice.ConsultantId,
                    SabreInvoiceId = TravelInvoice.SabreInvoiceId,
                    SubmittingUser = User.Identity.Name,
                    EventName = TravelInvoice.EventName,
                    ConsultantName = TravelInvoice.ConsultantName,
                    LineItems = lineItems,
                    WorkflowStatuses = workflowSteps
                }
            );

            var notification = TravelInvoice.GetNotification();

            return RedirectToPage("./Index", notification);
        }

        private IList<InvoiceFormViewModel.LineItem> GetLineItems()
        {
            var list = new List<InvoiceFormViewModel.LineItem>
            {
                new InvoiceFormViewModel.LineItem
                {
                    Id = 0,
                    TravelServiceId = 0,
                    Quantity = 1,
                    Amount = defaultAmount,
                    AmountAsString = defaultAmount.ToString("C2")
                }
            };

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
                        case "TravelServiceId":
                            item.LineItemError.TravelServiceValidationMessage = error.Item3.FirstOrDefault()?.ErrorMessage;
                            item.LineItemError.IsTravelServiceInvalid = true;

                            break;
                    }
                }
            }
        }
    }
}