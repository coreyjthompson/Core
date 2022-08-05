using System.Threading.Tasks;

using MEI.Core.DomainModels.Common;
using MEI.Core.Infrastructure.Data;
using MEI.Core.Infrastructure.Ldap.Queries;
using MEI.Core.Queries;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MEI.Web.Areas.Travel.Pages.Queues
{
    public class AwaitingSpeakerResponseModel : PageModel
    {
        private readonly CoreContext _context;
        private readonly IQueryProcessor _queries;


        public AwaitingSpeakerResponseModel(CoreContext context, IQueryProcessor queries)
        {
        }


        public IActionResult OnGet()
        {
            return Page();
        }

        //public async Task<IActionResult> OnPostAsync()
        //{
        //}

        private Task<ActiveDirectoryUser> GetUser()
        {
            var userId = User.Identity.Name;

            var query = new FindByIdentityQuery {Username = userId};

            return _queries.Execute(query);
        }
    }
}