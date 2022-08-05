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
    public class AddInvoiceCommand
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

    public class AddInvoiceCommandHandler
        : ICommandHandler<AddInvoiceCommand, int>
    {
        private readonly CoreContext _coreContext;
        private readonly IInvoiceService _invoiceService;
        private readonly IFeatureManager _featureManager;

        // AbbVie contexts
        private readonly Abbott2019Context _currentYearContextAbbvie;
        private readonly Abbott2018Context _previousYearContextAbbvie;

        private const int _currentYear = 2019;
        private const int _previousYear = 2018;

        public AddInvoiceCommandHandler(CoreContext coreContext, IInvoiceService invoiceService, Abbott2019Context currentYearContextAbbvie, Abbott2018Context previousYearContextAbbvie, IFeatureManager featureManager)
        {
            _coreContext = coreContext;
            _invoiceService = invoiceService;
            _featureManager = featureManager;
            _currentYearContextAbbvie = currentYearContextAbbvie;
            _previousYearContextAbbvie = previousYearContextAbbvie;
        }

        public async Task<int> HandleAsync(AddInvoiceCommand command)
        {
            // validate invoice data
            if (string.IsNullOrEmpty(command.ClientName))
            {
                throw new ArgumentNullException(nameof(command.ClientName));
            }

            var client = await _coreContext.Clients.FirstOrDefaultAsync(x => x.Name == command.ClientName);

            if (client == null)
            {
                throw new ArgumentException(string.Format("Invalid Client Name. {0}", command.ClientName));
            }

            if (command.LineItems.Any())
            {
                var services = await _coreContext.AgencyServices.ToListAsync();

                foreach (var line in command.LineItems)
                {
                    if (services.All(type => type.Id != line.AgencyServiceId))
                    {
                        throw new ArgumentException(string.Format("Invalid Travel Service Id. {0}", line.AgencyServiceId));
                    }
                }
            }

            // Add the new status
            var newList = new List<InvoiceWorkflowStatus>();

            foreach (var status in command.WorkflowStatuses)
            {
                var newStatus = new InvoiceWorkflowStatus {InvoiceId = command.InvoiceId, CreatedBy = command.SubmittingUser, WorkflowStepId = (int) status};
                newList.Add(newStatus);
            }

            var newInvoice = new Invoice
            {
                ClientId = client.Id,
                LineItems = command.LineItems,
                EventId = command.EventId,
                ConsultantId = command.ConsultantId,
                SabreInvoiceId = command.SabreInvoiceId,
                ConsultantName = command.ConsultantName,
                EventName = command.EventName,
                WorkflowSteps = newList
            };

            await _coreContext.TravelInvoices.AddAsync(newInvoice);
            await _coreContext.SaveChangesAsync();

            // If any of the statuses are of submit then we need to add the expense to the client database and upload into sharepoint
            if (command.WorkflowStatuses.Any(w => w == WorkflowStepEnum.InvoiceSubmittedForPayment))
            {

                if (_featureManager.IsEnabledAsync("ClientSpecificTravelInvoiceSubmissionCommands").Result)
                {
                    // Add expense records to the client's db
                    await AddClientSpecificRecords(command);
                }

                if (_featureManager.IsEnabledAsync("SharePointUploads").Result)
                {
                    // Add the invoice to client's sharepoint library
                    _invoiceService.AddInvoiceToSharePoint(newInvoice, client);
                }

            }

            return newInvoice.Id;
        }

        public async Task AddClientSpecificRecords(AddInvoiceCommand command)
        {
            switch (command.ClientName.ToLower())
            {
                case "abbvie":
                    await AddAbbVieTravelInvoiceExpenseRecords(command);

                    break;
            }
        }

        #region "AbbVie"
        private async Task AddAbbVieTravelInvoiceExpenseRecords(AddInvoiceCommand command)
        {

            // validate invoice data
            if (string.IsNullOrEmpty(command.EventName))
            {
                throw new ArgumentException(string.Format("Invalid Program Id. {0}", command.EventName));
            }

            var dbYear = Event.GetYearFromName(command.EventName);

            switch (dbYear)
            {
                case _currentYear:
                    await AddAbbVieCurrentYearRecords(command);
                    break;
                case _previousYear:
                    await AddAbbViePreviousYearRecords(command);
                    break;
                default:
                    throw new ArgumentException(string.Format("Invalid Year. {0}", dbYear));
            }

        }

        private async Task AddAbbVieCurrentYearRecords(AddInvoiceCommand command)
        {

            var program = await _currentYearContextAbbvie.Program.FirstOrDefaultAsync(x => x.ProgramId == command.EventName);

            if (program == null)
            {
                throw new ArgumentException(string.Format("Invalid Program Id. {0}", command.EventName));
            }

            if (command.ConsultantId == 0)
            {
                throw new ArgumentException(string.Format("Invalid Speaker Id/Counter. {0}", command.ConsultantId));
            }

            var speaker = await _currentYearContextAbbvie.Speaker.FirstOrDefaultAsync(x => x.SpkrCounter == command.ConsultantId);

            if (speaker == null)
            {
                throw new ArgumentException(string.Format("Invalid Speaker Id/Counter. {0}", command.ConsultantId));
            }

            //TODO: find out the drug using program name
            var drug = "test";

            foreach (var item in command.LineItems)
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
                    InvoiceNo = command.SabreInvoiceId,
                    Notes = string.Format("{2}/{0} {1}", speaker.Spkrfn, speaker.Spkrmi, speaker.Spkrln).Trim(),
                    Drug = drug,
                    SpkrCounter = speaker.SpkrCounter
                };

                await _currentYearContextAbbvie.Expenses.AddAsync(newExpense);
            }

            await _currentYearContextAbbvie.SaveChangesAsync();
        }

        private async Task AddAbbViePreviousYearRecords(AddInvoiceCommand command)
        {

            var program = await _previousYearContextAbbvie.Program.FirstOrDefaultAsync(x => x.ProgramId == command.EventName);

            if (program == null)
            {
                throw new ArgumentException(string.Format("Invalid Program Id. {0}", command.EventName));
            }

            if (command.ConsultantId == 0)
            {
                throw new ArgumentException(string.Format("Invalid Speaker Id/Counter. {0}", command.ConsultantId));
            }

            var speaker = await _previousYearContextAbbvie.Speaker.FirstOrDefaultAsync(x => x.SpkrCounter == command.ConsultantId);

            if (speaker == null)
            {
                throw new ArgumentException(string.Format("Invalid Speaker Id/Counter. {0}", command.ConsultantId));
            }

            //TODO: find out the drug using program name
            var drug = "test";

            foreach (var item in command.LineItems)
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
                    InvoiceNo = command.SabreInvoiceId,
                    Notes = string.Format("{2}/{0} {1}", speaker.Spkrfn, speaker.Spkrmi, speaker.Spkrln).Trim(),
                    Drug = drug,
                    SpkrCounter = speaker.SpkrCounter
                };

                //await _previousYearContextAbbvie.Expenses.AddAsync(newExpense);
            }
            await _previousYearContextAbbvie.SaveChangesAsync();

        }
        #endregion
    }
}