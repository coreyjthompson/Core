#region Using Statements

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

using MEI.Core.Commands;
using MEI.Core.DomainModels.Common;
using MEI.Core.DomainModels.Travel;
using MEI.Core.Infrastructure.Ldap.Queries;
using MEI.Core.Queries;
using MEI.Web.Models;
using MEI.Web.Models.Client;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

#endregion

namespace MEI.Web.Pages.Clients
{
    public class CreateModel : PageModel
    {
        private readonly IQueryProcessor _queries;
        private readonly ICommandProcessor _commands;

        public CreateModel(IQueryProcessor queries, ICommandProcessor commands)
        {
            _queries = queries;
            _commands = commands;

        }

        [BindProperty]
        public FormViewModel ClientForm { get; set; } = new FormViewModel();

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            //return RedirectToPage("./Index", notification);
            return RedirectToPage("./Index");
        }

        private Task<ActiveDirectoryUser> GetUser()
        {
            var userId = User.Identity.Name;

            var query = new FindByIdentityQuery
            {
                Username = userId
            };

            return _queries.Execute(query);
        }

    }
}