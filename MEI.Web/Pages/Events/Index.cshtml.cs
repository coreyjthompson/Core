using System;
using System.Collections.Generic;
using System.Linq;

using MEI.Core.Queries;
using MEI.Web.Models;

using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MEI.Web.Pages.Events
{
    public class IndexModel : PageModel
    {
        private readonly IQueryProcessor _queries;

        public IndexModel(IQueryProcessor queries)
        {
            _queries = queries;
        }

        public void OnGet(NotificationViewModel notification)
        {
        }
    }
}
