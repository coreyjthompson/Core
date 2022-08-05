using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using MEI.Core.Commands;
using MEI.Core.DomainModels.Travel;
using MEI.Core.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;

namespace MEI.Travel.Commands
{
    public class Demo_AddInvoiceCommand
        : ICommand<int>
    {
        public string ClientName { get; set; }

        public int InvoiceId { get; set; }

        public IList<InvoiceLineItem> Expenses { get; set; }

        public override string ToString()
        {
            return string.Format("[InvoiceId={0}, ClientName={1}, ExpenseCount={2}]", InvoiceId, ClientName, Expenses?.Count);
        }
    }

    public class Demo_AddInvoiceCommandHandler
        : ICommandHandler<Demo_AddInvoiceCommand, int>
    {
        private readonly CoreContext _db;

        public Demo_AddInvoiceCommandHandler(CoreContext db)
        {
            _db = db;
        }

        public async Task<int> HandleAsync(Demo_AddInvoiceCommand command)
        {
            // validate invoice data

            if (string.IsNullOrEmpty(command.ClientName))
            {
                throw new ArgumentNullException(nameof(command.ClientName));
            }

            var client = await _db.Clients.FirstOrDefaultAsync(x => x.Name == command.ClientName);

            if (client == null)
            {
                throw new ArgumentException(string.Format("Invalid ClientName. {0}", command.ClientName));
            }

            var newInvoice = new Invoice {ClientId = client.Id, LineItems = command.Expenses};

            await _db.TravelInvoices.AddAsync(newInvoice);

            await _db.SaveChangesAsync();

            return newInvoice.Id;

            // create invoice document

            // save invoice document to SharePoint

            // write log pertaining to invoice add
        }
    }
}