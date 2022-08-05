using System.Security.Claims;
using System.Threading.Tasks;

using MEI.Core.DomainModels.Common;
using MEI.Core.Infrastructure.Ldap.Queries;
using MEI.Core.Queries;

using Microsoft.AspNetCore.Authentication;

namespace MEI.Web
{
    public class ClaimsTransformer
        : IClaimsTransformation
    {
        private readonly IQueryProcessor _queries;

        public ClaimsTransformer(IQueryProcessor queries)
        {
            _queries = queries;
        }

        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            ActiveDirectoryUser user = await _queries.Execute(new FindByIdentityQuery
                                        {
                                            Username = principal.Identity.Name
                                        });

            if (user == null)
            {
                return principal;
            }

            ((ClaimsIdentity)principal.Identity).AddClaims(new[]
                                                           {
                                                               new Claim(ClaimTypes.GivenName, user.FirstName),
                                                               new Claim(ClaimTypes.Surname, user.LastName),
                                                               new Claim(ClaimTypes.Email, user.EmailAddress)
                                                           });

            if(user.FirstName.ToLower() == "corey" && user.LastName.ToLower() == "thompson")
            {
                ((ClaimsIdentity)principal.Identity).AddClaims(new[]
                                                           {
                                                               new Claim("IsCoreyThompson", "true")
                                                           });

            }

            return principal;
        }
    }
}
