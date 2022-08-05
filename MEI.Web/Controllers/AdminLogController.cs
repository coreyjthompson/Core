using MEI.Core.Infrastructure.Admin.Queries;
using MEI.Core.Infrastructure.Queries;
using MEI.Core.Queries;
using MEI.Web.Models.AdminLog;

using Microsoft.AspNetCore.Mvc;

namespace MEI.Web.Controllers
{
    public class AdminLogController
        : Controller
    {
        private readonly IQueryProcessor _queries;

        public AdminLogController(IQueryProcessor queries)
        {
            _queries = queries;
        }

        public IActionResult List(string applicationName, int pageIndex, int pageSize)
        {
            var query = new Demo_GetLogsForApplicationQuery
                        {
                            ApplicationName = applicationName,
                            Environment = "LocalDev",
                            Paging = new PageInfo
                                     {
                                         PageIndex = pageIndex,
                                         PageSize = pageSize
                                     }
                        };
            var logs = _queries.Execute(query);

            return View(new ListViewModel
                        {
                            Logs = logs.Result.Items
                        });
        }
    }
}
