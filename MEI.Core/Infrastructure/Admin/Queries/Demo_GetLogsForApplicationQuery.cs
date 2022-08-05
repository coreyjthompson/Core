using System.ComponentModel.DataAnnotations;
using System.Linq;

using MEI.Core.DomainModels.Common;
using MEI.Core.Infrastructure.Data;
using MEI.Core.Infrastructure.Data.Helpers;
using MEI.Core.Infrastructure.Queries;
using MEI.Core.Queries;

namespace MEI.Core.Infrastructure.Admin.Queries
{
    public class Demo_GetLogsForApplicationQuery
        : IQuery<Paged<Log>>
    {
        [Required]
        public string ApplicationName { get; set; }

        /// <summary>
        /// The environment of the logs to return. Defaults to "Production".
        /// </summary>
        public string Environment { get; set; } = "Production";

        public PageInfo Paging { get; set; }

        public override string ToString()
        {
            return string.Format("[ApplicationName={0}, Environment={1}, Paging.PageIndex={2}, Paging.PageSize={3}]",
                ApplicationName,
                Environment,
                Paging?.PageIndex,
                Paging?.PageSize);
        }
    }

    public class Demo_GetLogsForApplicationQueryHandler
        : IQueryHandler<Demo_GetLogsForApplicationQuery, Paged<Log>>
    {
        private readonly CoreContext _db;

        public Demo_GetLogsForApplicationQueryHandler(CoreContext db)
        {
            _db = db;
        }

        public System.Threading.Tasks.Task<Paged<Log>> HandleAsync(Demo_GetLogsForApplicationQuery query)
        {
            return _db.Logs.Where(x => x.Logger.StartsWith(query.ApplicationName) && x.Environment == query.Environment)
                .OrderByDescending(x => x.WhenLogged)
                .Page(query.Paging);
        }
    }
}
