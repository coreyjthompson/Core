using System.Diagnostics;

using MEI.Core.Infrastructure.Ldap.Queries;
using MEI.Core.Queries;
using MEI.Web.Models;

using Microsoft.AspNetCore.Mvc;

namespace MEI.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IQueryProcessor _queries;
        private readonly bool isLdapAvailable = true;

        public HomeController(IQueryProcessor queries)
        {
            _queries = queries;
        }

        public IActionResult Index()
        {
            if (isLdapAvailable)
            {
                var userId = User.Identity.Name;

                var query = new FindByIdentityQuery { Username = userId };

                var user = _queries.Execute(query).Result;

                if (user.Groups.Contains("Travel") || user.Groups.Contains("Domain Admins") || user.Username.ToLower().Contains("jparker"))
                {
                    return LocalRedirect("/Travel/");
                }
            }

            return LocalRedirect("/Travel/");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}