using System;
using System.Collections;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Threading.Tasks;

using MEI.Core.DomainModels.Common;
using MEI.Core.Infrastructure.Queries;

using Microsoft.Extensions.Options;

namespace MEI.Core.Infrastructure.Ldap.Queries
{
    public class FindByOrganizationalUnitQuery
        : IQuery<List<ActiveDirectoryUser>>, ICacheQuery
    {
        public string GroupName { get; set; }

        public CacheQueryOptions CacheQueryOptions =>
            new CacheQueryOptions {CacheKey = string.Format("FindByOrganizationalUnitQuery-{0}", ToString()), SlidingExpiration = TimeSpan.FromMinutes(5)};

        public override string ToString()
        {
            return string.Format("[GroupName={0}]", GroupName);
        }
    }

    public class FindByOrganizationalUnitQueryHandler
        : IQueryHandler<FindByOrganizationalUnitQuery, List<ActiveDirectoryUser>>
    {
        private readonly InfrastructureOptions _options;

        public FindByOrganizationalUnitQueryHandler(IOptions<InfrastructureOptions> options)
        {
            _options = options.Value;
        }

        public Task<List<ActiveDirectoryUser>> HandleAsync(FindByOrganizationalUnitQuery query)
        {
            List<ActiveDirectoryUser> allUsers = new List<ActiveDirectoryUser>();
            // Create the container for LDAP
            var container = $"OU={query.GroupName},DC=meintl,DC=com";
            using (var context = new PrincipalContext(ContextType.Domain, _options.LdapIpAddress, container))
            {
                UserPrincipal qbeUser = new UserPrincipal(context);

                // Create your principal searcher passing in the user principal    
                PrincipalSearcher srch = new PrincipalSearcher(qbeUser);
                // find all matches
                foreach (var found in srch.FindAll())
                {
                    var user = new ActiveDirectoryUser
                    {
                        Username = found.SamAccountName,
                        DisplayName = found.DisplayName,
                    };

                    allUsers.Add(user);
                }
            }

            return Task.FromResult(allUsers);

        }
    }
}