using System;
using System.Threading.Tasks;

using MEI.Core.Commands;
using MEI.Core.DomainModels.Travel;
using MEI.Core.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;

namespace MEI.Travel.Commands
{
    public class EditAgencyServiceCommand
        : ICommand<int>
    {
        public int  Id { get; set; }

        public string Name { get; set; }

        public decimal FeeAmount { get; set; }

        public bool IsInactive { get; set; }

        public int SortOrder { get; set; }

        public override string ToString()
        {
            return string.Format("[Id={3}, Name={0}, FeeAmount={1}, SortOrder={2}", Name, FeeAmount,  SortOrder, Id );
        }
    }

    public class EditAgencyServiceCommandHandler
        : ICommandHandler<EditAgencyServiceCommand, int>
    {
        private readonly CoreContext _coreContext;

        public EditAgencyServiceCommandHandler(CoreContext coreContext)
        {
            _coreContext = coreContext;
        }

        public async Task<int> HandleAsync(EditAgencyServiceCommand command)
        {
            if(command.Id == 0)
            {
                throw new ArgumentNullException(nameof(command.Id));
            }

            var service = await _coreContext.AgencyServices.FirstOrDefaultAsync(s => s.Id == command.Id);

            if (service == null)
            {
                throw new ArgumentException(string.Format("Invalid Agency Service Id. {0}", command.Id));
            }

            // get the currency id for USD
            var currency = await _coreContext.Currencies.FirstOrDefaultAsync(c => c.IsoSymbol.ToLower() == "usd");

            if (currency == null)
            {
                throw new ArgumentException("Cannot find currency id for USD.");
            }

            service.Name = command.Name;
            service.FeeAmount = command.FeeAmount;
            service.FeeCurrencyId = currency.Id;
            service.SortOrder = command.SortOrder;
            //TODO: add inactivation

            await _coreContext.SaveChangesAsync();

            // Return the new id
            return service.Id;
        }
    }
}