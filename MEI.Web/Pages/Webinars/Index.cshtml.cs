using MEI.Core.Queries;
using MEI.Web.Models;

using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MEI.Web.Pages.Webinars
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
