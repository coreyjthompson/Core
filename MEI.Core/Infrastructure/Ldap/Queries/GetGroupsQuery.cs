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
    public class GetGroupsQuery
        : IQuery<List<string>>, ICacheQuery
    {

        public CacheQueryOptions CacheQueryOptions =>
            new CacheQueryOptions {CacheKey = string.Format("GetGroupsQuery", ToString()), SlidingExpiration = TimeSpan.FromMinutes(5)};

    }

    public class GetGroupsQueryHandler
        : IQueryHandler<GetGroupsQuery, List<string>>
    {
        private readonly InfrastructureOptions _options;

        public GetGroupsQueryHandler(IOptions<InfrastructureOptions> options)
        {
            _options = options.Value;
        }

        public Task<List<string>> HandleAsync(GetGroupsQuery query)
        {
            var groups = new List<string>();
            PrincipalContext ctx = new PrincipalContext(ContextType.Domain);

            // define a "query-by-example" principal - here, we search for a GroupPrincipal 
            GroupPrincipal qbeGroup = new GroupPrincipal(ctx);

            // create your principal searcher passing in the QBE principal    
            PrincipalSearcher srch = new PrincipalSearcher(qbeGroup);

            // find all matches
            foreach (var result in srch.FindAll())
            {
                groups.Add(result.Name);
            }

            return Task.FromResult(groups);

        }
    }
}