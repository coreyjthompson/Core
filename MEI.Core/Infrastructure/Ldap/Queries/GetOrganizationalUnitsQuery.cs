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
    public class GetOrganizationalUnitsQuery
        : IQuery<List<string>>, ICacheQuery
    {

        public CacheQueryOptions CacheQueryOptions =>
            new CacheQueryOptions {CacheKey = string.Format("GetOrganizationalUnitsQuery", ToString()), SlidingExpiration = TimeSpan.FromMinutes(5)};

    }

    public class GetOrganizationalUnitsQueryHandler
        : IQueryHandler<GetOrganizationalUnitsQuery, List<string>>
    {
        private readonly InfrastructureOptions _options;

        public GetOrganizationalUnitsQueryHandler(IOptions<InfrastructureOptions> options)
        {
            _options = options.Value;
        }

        public Task<List<string>> HandleAsync(GetOrganizationalUnitsQuery query)
        {

            List<string> orgUnits = new List<string>();

            DirectoryEntry startingPoint = new DirectoryEntry("LDAP://DC=meintl,DC=com");

            DirectorySearcher searcher = new DirectorySearcher(startingPoint);
            searcher.Filter = "(objectCategory=organizationalUnit)";

            foreach (SearchResult res in searcher.FindAll())
            {
                orgUnits.Add(res.Path);
            }

            return Task.FromResult(orgUnits);
        }
    }
}