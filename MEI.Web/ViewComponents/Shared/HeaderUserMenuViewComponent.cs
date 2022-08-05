using System.Threading.Tasks;

using MEI.Core.DomainModels.Common;
using MEI.Core.Infrastructure.Ldap.Queries;
using MEI.Core.Queries;

using Microsoft.AspNetCore.Mvc;

namespace MEI.Web.ViewComponents.Shared
{
    public class HeaderUserMenuViewComponent : ViewComponent
    {
        private readonly bool _isLdapAvailable = true;
        private readonly IQueryProcessor _queries;

        public HeaderUserMenuViewComponent(IQueryProcessor queries)
        {
            _queries = queries;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userId = User.Identity.Name;
            var user = new ActiveDirectoryUser();
            if (userId == null)
            {
                return View(user);
            }

            var query = new FindByIdentityQuery {Username = userId};

            if (_isLdapAvailable)
            {
                user = await _queries.Execute(query) ?? new ActiveDirectoryUser();
            }

            return View(user);
        }
    }
}