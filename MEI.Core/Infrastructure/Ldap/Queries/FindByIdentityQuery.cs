using System;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Threading.Tasks;

using MEI.Core.DomainModels.Common;
using MEI.Core.Infrastructure.Queries;

using Microsoft.Extensions.Options;

namespace MEI.Core.Infrastructure.Ldap.Queries
{
    public class FindByIdentityQuery
        : IQuery<ActiveDirectoryUser>, ICacheQuery
    {
        public string Username { get; set; }

        public CacheQueryOptions CacheQueryOptions =>
            new CacheQueryOptions {CacheKey = string.Format("FindByIdentityQuery-{0}", ToString()), SlidingExpiration = TimeSpan.FromMinutes(5)};

        public override string ToString()
        {
            return string.Format("[Username={0}]", Username);
        }
    }

    public class FindByIdentityQueryHandler
        : IQueryHandler<FindByIdentityQuery, ActiveDirectoryUser>
    {
        private readonly InfrastructureOptions _options;

        public FindByIdentityQueryHandler(IOptions<InfrastructureOptions> options)
        {
            _options = options.Value;
        }

        public Task<ActiveDirectoryUser> HandleAsync(FindByIdentityQuery query)
        {
            // Create the context using ldap url
            using (var context = new PrincipalContext(ContextType.Domain, _options.LdapIpAddress))
            {
                // Find the user in ldap
                var user = UserPrincipal.FindByIdentity(context, query.Username);

                if (user == null)
                {
                    return Task.FromResult<ActiveDirectoryUser>(null);
                }

                // Fill the user model
                var current = new ActiveDirectoryUser
                {
                    Username = user.SamAccountName,
                    EmailAddress = user.EmailAddress,
                    DisplayName = user.DisplayName,
                    FirstName = user.GivenName,
                    LastName = user.Surname
                };

                // Get all of the user's AD groups
                foreach (var group in user.GetGroups())
                {
                    current.Groups.Add(group.Name);
                }

                // if de is not null
                if (user.GetUnderlyingObject() is DirectoryEntry de)
                {
                    if (de.Properties["thumbnailPhoto"].Value is byte[] data)
                    {
                        current.AvatarSource = $"data:image;base64,{Convert.ToBase64String(data)}";
                    }
                    else
                    {
                        current.AvatarSource = _options.DefaultUserAvatarImagePath;
                    }
                }

                return Task.FromResult(current);
            }
        }
    }
}