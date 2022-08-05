using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

using MEI.Core.DomainModels.Travel;
using MEI.Core.Infrastructure.Data;
using MEI.Core.Infrastructure.Data.Helpers;
using MEI.Core.Infrastructure.Queries;

using Microsoft.EntityFrameworkCore;

namespace MEI.Travel.Queries
{
    public class Demo_GetAllInvoicesForClientQuery
        : IQuery<Paged<Invoice>>
    {
        [Required] public string ClientName { get; set; }

        public PageInfo Paging { get; set; }

        public override string ToString()
        {
            return string.Format("[ClientName={0}, Paging.PageIndex={1}, Paging.PageSize={2}]", ClientName, Paging?.PageIndex, Paging?.PageSize);
        }
    }

    public class Demo_GetAllInvoicesForClientQueryHandler
        : IQueryHandler<Demo_GetAllInvoicesForClientQuery, Paged<Invoice>>
    {
        private readonly CoreContext _db;

        public Demo_GetAllInvoicesForClientQueryHandler(CoreContext db)
        {
            _db = db;
        }

        public Task<Paged<Invoice>> HandleAsync(Demo_GetAllInvoicesForClientQuery query)
        {
            return _db.TravelInvoices
                .Include("Client")
                .Include("Expenses")
                .Where(x => x.Client.Name == query.ClientName)
                .OrderBy(x => x.Id)
                .Page(query.Paging);
        }
    }
}