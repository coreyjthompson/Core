using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Threading.Tasks;

using MEI.Core.DomainModels.Common;
using MEI.Core.Infrastructure.Queries;

using Microsoft.Extensions.Options;

namespace MEI.Core.Infrastructure.Ldap.Queries
{
    public class FindAllDirectoryEntriesByUserQuery
        : IQuery<List<(string, string)>>, ICacheQuery
    {
        public string Username { get; set; }

        public CacheQueryOptions CacheQueryOptions =>
            new CacheQueryOptions {CacheKey = string.Format("FindAllDirectoryEntriesByUserQuery-{0}", ToString()), SlidingExpiration = TimeSpan.FromMinutes(5)};

        public override string ToString()
        {
            return string.Format("[Username={0}]", Username);
        }
    }

    public class FindAllDirectoryEntriesByUserQueryHandler
        : IQueryHandler<FindAllDirectoryEntriesByUserQuery, List<(string, string)>>
    {
        private readonly InfrastructureOptions _options;

        public FindAllDirectoryEntriesByUserQueryHandler(IOptions<InfrastructureOptions> options)
        {
            _options = options.Value;
        }

        public Task<List<(string, string)>> HandleAsync(FindAllDirectoryEntriesByUserQuery query)
        {
            // Create the context using ldap url
            using (var context = new PrincipalContext(ContextType.Domain, _options.LdapIpAddress))
            {
                // Find the user in ldap
                var user = UserPrincipal.FindByIdentity(context, query.Username);
                var tupleList = new List<(string, string)>();
                if (user == null)
                {
                    return Task.FromResult<List<(string, string)>>(null);
                }

                // if de is not null
                if (user.GetUnderlyingObject() is DirectoryEntry de)
                {
                    foreach (var property in de.Properties.PropertyNames)
                    {
                        tupleList.Add((property.ToString(), de.Properties[property.ToString()].Value.ToString()));
                    }
                }

                return Task.FromResult(tupleList); 
            }
        }
    }
}