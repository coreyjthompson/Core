using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MEI.Core.Commands;
using MEI.Core.DomainModels.Travel;
using MEI.Core.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;

namespace MEI.Travel.Commands
{
    public class Demo_EditInvoiceCommand
        : ICommand<int>
    {
        public int InvoiceId { get; set; }

        public string ClientName { get; set; }

        public IList<(string travelService, decimal amount)> Expenses { get; set; }

        public override string ToString()
        {
            return string.Format("[InvoiceId={0}, ClientName={1}, ExpenseCount={2}]", InvoiceId, ClientName, Expenses?.Count);
        }
    }

    public class Demo_EditInvoiceCommandHandler
        : ICommandHandler<Demo_EditInvoiceCommand, int>
    {
        private readonly CoreContext _db;

        public Demo_EditInvoiceCommandHandler(CoreContext db)
        {
            _db = db;
        }

        public async Task<int> HandleAsync(Demo_EditInvoiceCommand command)
        {
            // These two tasks can run in parallel because their only dependency in from the input parameter
            // So, create the tasks first but do not await them yet. This will cause them to each start executing.

            var invoiceTask = _db.TravelInvoices
                .Include("Expenses")
                .Include("Client")
                .FirstOrDefaultAsync(x => x.Id == command.InvoiceId);

            var clientTask = _db.Clients.FirstOrDefaultAsync(x => x.Name == command.ClientName);

            // Now, await them so that they all must be completed before any code after this can run which is
            // dependent on them
            await Task.WhenAll(invoiceTask, clientTask);
            // or
            // var invoice = await invoiceTask;
            // var client = await clientTask;

            if (invoiceTask.Result == null)
            {
                throw new ArgumentException("Invalid invoice id. " + command.InvoiceId);
            }

            var invoice = invoiceTask.Result;
            invoice.Client = clientTask.Result;

            if (command.Expenses == null)
            {
                invoice.LineItems = null;
            }
            else
            {
                invoice.LineItems = command.Expenses.Select(tuple => new InvoiceLineItem {Amount = tuple.amount, AgencyService = _db.AgencyServices.FirstOrDefault(x => x.Name == tuple.travelService)}).ToList();
            }

            await _db.SaveChangesAsync();

            return invoice.Id;
        }
    }
}