using System.Collections.Generic;

using MEI.Core.DomainModels.Common;
using MEI.Core.DomainModels.Travel;

namespace MEI.Travel.Services
{
    public interface IInvoiceService
    {
        void AddInvoiceToSharePoint(Invoice invoice, Client client);
    }
}