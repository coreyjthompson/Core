using System.Collections.Generic;
using System.Threading.Tasks;

using MEI.Core.Infrastructure;
using MEI.Core.Infrastructure.Ldap.Queries;
using MEI.Core.Queries;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace MEI.Web.Areas.Admin.Pages.Users
{
    public class DirectoryEntryListModel : PageModel
    {
        private readonly InfrastructureOptions _options;
        private readonly IQueryProcessor _queries;

        public DirectoryEntryListModel(IQueryProcessor queries, IOptions<InfrastructureOptions> options)
        {
            _queries = queries;
            _options = options.Value;
        }

        public IList<(string, string)> Entries { get; set; } = new List<(string, string)>();

        public void OnGet()
        {
            var userId = User.Identity.Name;
            var query = new FindAllDirectoryEntriesByUserQuery {Username = userId};
            Entries = _queries.Execute(query).Result;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            return Page();
        }
    }
}