using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MEI.AbbVie.Infrastructure.Data.Models2018;
using MEI.AbbVie.Infrastructure.Data.Models2019;
using MEI.Core.Commands;
using MEI.Core.DomainModels;
using MEI.Core.DomainModels.Common;
using MEI.Core.DomainModels.Travel;
using MEI.Core.Infrastructure.Data;
using MEI.Travel.Services;

using Microsoft.EntityFrameworkCore;
using Microsoft.FeatureManagement;

namespace MEI.Travel.Commands
{
    public class EditInvoiceCommand
        : ICommand<int>
    {
        public int InvoiceId { get; set; }

        public string SabreInvoiceId { get; set; }

        public int? EventId { get; set; }

        public int? ConsultantId { get; set; }

        public string ClientName { get; set; }

        public string SubmittingUser { get; set; }

        public string ConsultantName { get; set; }

        public string EventName { get; set; }

        public IList<InvoiceLineItem> LineItems { get; set; } = new List<InvoiceLineItem>();

        [Required] public IList<WorkflowStepEnum> WorkflowStatuses { get; set; }

        public override string ToString()
        {
            return string.Format("[InvoiceId={0}, SabreId={1}, EventId={2}, ConsultantId={3}, ClientName={4}, SubmittingUser={5}, ConsultantName={6}, EventName={7}, LineItemsCount={8}, WorkflowStatusesCount={9}]", InvoiceId, SabreInvoiceId, EventId, ConsultantId, ClientName, SubmittingUser, ConsultantName, EventName, LineItems?.Count(), WorkflowStatuses.Count());
        }
    }

    public class EditInvoiceCommandHandler
        : ICommandHandler<EditInvoiceCommand, int>
    {
        private readonly CoreContext _coreContext;
        private readonly IInvoiceService _invoiceService;
        private readonly IFeatureManager _featureManager;

        // AbbVie contexts
        private readonly Abbott2019Context _currentYearContextAbbvie;
        private readonly Abbott2018Context _previousYearContextAbbvie;

        private const int _currentYear = 2019;
        private const int _previousYear = 2018;


        public EditInvoiceCommandHandler(CoreContext coreContext, IInvoiceService invoiceService, Abbott2019Context currentYearContextAbbvie, Abbott2018Context previousYearContextAbbvie, IFeatureManager featureManager)
        {
            _coreContext = coreContext;
            _invoiceService = invoiceService;
            _currentYearContextAbbvie = currentYearContextAbbvie;
            _previousYearContextAbbvie = previousYearContextAbbvie;
        }

        public async Task<int> HandleAsync(EditInvoiceCommand command)
        {
            // These two tasks can run in parallel because their only dependency in from the input parameter
            // So, create the tasks first but do not await them yet. This will cause them to each start executing.

            var invoiceTask = _coreContext.TravelInvoices
                .Include("LineItems")
                .Include("LineItems.AgencyService")
                .Include("Client")
                .FirstOrDefaultAsync(x => x.Id == command.InvoiceId);

            if (string.IsNullOrEmpty(command.ClientName))
            {
                throw new ArgumentNullException(nameof(command.ClientName));
            }

            var clientTask = _coreContext.Clients.FirstOrDefaultAsync(x => x.Name == command.ClientName);

            await Task.WhenAll(invoiceTask, clientTask);

            if (invoiceTask.Result == null)
            {
                throw new ArgumentException(string.Format("Invalid Invoice Id. {0}", command.InvoiceId));
            }

            if (clientTask == null)
            {
                throw new ArgumentException(string.Format("Invalid Client Name. {0}", command.ClientName));
            }

            var invoice = invoiceTask.Result;
            var client = clientTask.Result;

            invoice.ClientId = client.Id;
            invoice.EventId = command.EventId;
            invoice.EventName = command.EventName;
            invoice.SabreInvoiceId = command.SabreInvoiceId;
            invoice.ConsultantId = command.ConsultantId;
            invoice.ConsultantName = command.ConsultantName;

            // Add the new statuses
            foreach (var status in command.WorkflowStatuses)
            {
                var newStatus = new InvoiceWorkflowStatus {InvoiceId = command.InvoiceId, CreatedBy = command.SubmittingUser, WorkflowStepId = (int) status};
                invoice.WorkflowSteps.Add(newStatus);
            }

            if (command.LineItems == null || !command.LineItems.Any())
            {
                invoice.LineItems = new List<InvoiceLineItem>();
            }
            else
            {
                var services = await _coreContext.AgencyServices.ToListAsync();
                foreach (var item in command.LineItems)
                {
                    // Validate the service and make sure it exists
                    if (services.All(type => type.Id != item.AgencyServiceId))
                    {
                        throw new ArgumentException(string.Format("Invalid Travel Service Id. {0}", item.AgencyServiceId));
                    }

                    // delete unwanted items
                    foreach (var existingChild in invoice.LineItems.ToList())
                    {
                        if (!command.LineItems.Any(c => c.Id == existingChild.Id))
                        {
                            invoice.LineItems.Remove(existingChild);
                        }
                    }

                    // Update the line item itself
                    var existingItem = invoice.LineItems.Where(c => c.Id == item.Id).FirstOrDefault();
                    if (existingItem != null && existingItem.Id != 0)
                    {
                        existingItem.Quantity = item.Quantity;
                        existingItem.AgencyServiceId = item.AgencyServiceId;
                        existingItem.Amount = item.Amount;
                    }                      
                    else
                    {
                        // If it wasn't found in the list then we need to add it
                        invoice.LineItems.Add(item);
                    }

                }
            }

            await _coreContext.SaveChangesAsync();

            // If any of the statuses are of submit then we need to add the expense to the client database and upload into sharepoint
            if (command.WorkflowStatuses.Any(w => w == WorkflowStepEnum.InvoiceSubmittedForPayment))
            {
                if (_featureManager.IsEnabledAsync("ClientSpecificTravelInvoiceSubmissionCommands").Result)
                {
                    // Add expense records to the client's db
                    await AddClientSpecificRecords(invoice);
                }

                if (_featureManager.IsEnabledAsync("SharePointUploads").Result)
                {
                    // Add the invoice to client's sharepoint library
                    _invoiceService.AddInvoiceToSharePoint(invoice, client);
                }
            }

            return invoice.Id;
        }

        public async Task AddClientSpecificRecords(Invoice invoice)
        {
            switch (invoice.Client.Name.ToLower())
            {
                case "abbvie":
                    await AddAbbVieTravelInvoiceExpenseRecords(invoice);

                    break;
            }
        }

        #region "AbbVie"
        private async Task AddAbbVieTravelInvoiceExpenseRecords(Invoice invoice)
        {

            // validate invoice data
            if (string.IsNullOrEmpty(invoice.EventName))
            {
                throw new ArgumentException(string.Format("Invalid Program Id. {0}", invoice.EventName));
            }

            var dbYear = Event.GetYearFromName(invoice.EventName);

            switch (dbYear)
            {
                case _currentYear:
                    await AddAbbVieCurrentYearRecords(invoice);
                    break;
                case _previousYear:
                    await AddAbbViePreviousYearRecords(invoice);
                    break;
                default:
                    throw new ArgumentException(string.Format("Invalid Year. {0}", dbYear));
            }

        }

        private async Task AddAbbVieCurrentYearRecords(Invoice invoice)
        {

            var program = await _currentYearContextAbbvie.Program.FirstOrDefaultAsync(x => x.ProgramId == invoice.EventName);

            if (program == null)
            {
                throw new ArgumentException(string.Format("Invalid Program Id. {0}", invoice.EventName));
            }

            if (invoice.ConsultantId == 0)
            {
                throw new ArgumentException(string.Format("Invalid Speaker Id/Counter. {0}", invoice.ConsultantId));
            }

            var speaker = await _currentYearContextAbbvie.Speaker.FirstOrDefaultAsync(x => x.SpkrCounter == invoice.ConsultantId);

            if (speaker == null)
            {
                throw new ArgumentException(string.Format("Invalid Speaker Id/Counter. {0}", invoice.ConsultantId));
            }

            //TODO: find out the drug using program name
            var drug = "test";

            foreach (var item in invoice.LineItems)
            {
                var newExpense = new MEI.AbbVie.Infrastructure.Data.Models2019.Expenses()
                {
                    ProgramId = program.ProgramId,
                    ExpenseAmt = item.Amount,
                    EstimatedAmt = item.Amount,
                    ExpenseType = "Travel Booking",
                    Hitdate = null,
                    Enterdate = DateTime.Now,
                    Description = string.Format("Agency Service Fee for {0}", item.AgencyService.Name).ToUpper(),
                    InvoiceNo = invoice.SabreInvoiceId,
                    Notes = string.Format("{2}/{0} {1}", speaker.Spkrfn, speaker.Spkrmi, speaker.Spkrln).Trim(),
                    Drug = drug,
                    SpkrCounter = speaker.SpkrCounter
                };

                await _currentYearContextAbbvie.Expenses.AddAsync(newExpense);
            }

            await _currentYearContextAbbvie.SaveChangesAsync();
        }

        private async Task AddAbbViePreviousYearRecords(Invoice invoice)
        {

            var program = await _previousYearContextAbbvie.Program.FirstOrDefaultAsync(x => x.ProgramId == invoice.EventName);

            if (program == null)
            {
                throw new ArgumentException(string.Format("Invalid Program Id. {0}", invoice.EventName));
            }

            if (invoice.ConsultantId == 0)
            {
                throw new ArgumentException(string.Format("Invalid Speaker Id/Counter. {0}", invoice.ConsultantId));
            }

            var speaker = await _previousYearContextAbbvie.Speaker.FirstOrDefaultAsync(x => x.SpkrCounter == invoice.ConsultantId);

            if (speaker == null)
            {
                throw new ArgumentException(string.Format("Invalid Speaker Id/Counter. {0}", invoice.ConsultantId));
            }

            //TODO: find out the drug using program name
            var drug = "test";

            foreach (var item in invoice.LineItems)
            {
                var newExpense = new MEI.AbbVie.Infrastructure.Data.Models2019.Expenses()
                {
                    ProgramId = program.ProgramId,
                    ExpenseAmt = item.Amount,
                    EstimatedAmt = item.Amount,
                    ExpenseType = "Travel Booking",
                    Hitdate = null,
                    Enterdate = DateTime.Now,
                    Description = string.Format("Agency Service Fee for {0}", item.AgencyService.Name).ToUpper(),
                    InvoiceNo = invoice.SabreInvoiceId,
                    Notes = string.Format("{2}/{0} {1}", speaker.Spkrfn, speaker.Spkrmi, speaker.Spkrln).Trim(),
                    Drug = drug,
                    SpkrCounter = speaker.SpkrCounter
                };

                //await _previousYearContextAbbvie.Expenses.AddAsync(newExpense);
            }
            await _previousYearContextAbbvie.SaveChangesAsync();

        }

    }
    #endregion

}