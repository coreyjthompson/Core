using System;
using System.Threading.Tasks;

using MEI.Core.Commands;
using MEI.Core.DomainModels.Travel;
using MEI.Core.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;

namespace MEI.Travel.Commands
{
    public class AddAgencyServiceCommand
        : ICommand<int>
    {
        public string Name { get; set; }

        public decimal FeeAmount { get; set; }

        public bool IsInactive { get; set; }

        public int SortOrder { get; set; }

        public override string ToString()
        {
            return string.Format("[Name={0}, FeeAmount={1}, SortOrder={2}", Name, FeeAmount, SortOrder);
        }
    }

    public class AddAgencyServiceCommandHandler
        : ICommandHandler<AddAgencyServiceCommand, int>
    {
        private readonly CoreContext _coreContext;

        public AddAgencyServiceCommandHandler(CoreContext coreContext)
        {
            _coreContext = coreContext;
        }

        public async Task<int> HandleAsync(AddAgencyServiceCommand command)
        {
            // get the currency id for USD
            var currency = await _coreContext.Currencies.FirstOrDefaultAsync(c => c.IsoSymbol.ToLower() == "usd");

            if (currency == null)
            {
                throw new ArgumentException("Cannot find currency id for USD.");
            }

            var newService = new AgencyService
            {
                Name = command.Name,
                FeeAmount = command.FeeAmount,
                FeeCurrencyId = currency.Id,
                //TODO: add inactivation
            };

            await _coreContext.AgencyServices.AddAsync(newService);
            await _coreContext.SaveChangesAsync();

            // Return the new id
            return newService.Id;
        }
    }
}