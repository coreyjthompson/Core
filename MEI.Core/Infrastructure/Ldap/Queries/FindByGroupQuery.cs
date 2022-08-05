using System;
using System.Collections;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MEI.Core.DomainModels.Common;
using MEI.Core.Infrastructure.Queries;

using Microsoft.Extensions.Options;

namespace MEI.Core.Infrastructure.Ldap.Queries
{
    public class FindByGroupQuery
        : IQuery<List<ActiveDirectoryUser>>, ICacheQuery
    {
        public string GroupName { get; set; }

        public CacheQueryOptions CacheQueryOptions =>
            new CacheQueryOptions { CacheKey = string.Format("FindByGroupQuery-{0}", ToString()), SlidingExpiration = TimeSpan.FromMinutes(5) };

        public override string ToString()
        {
            return string.Format("[GroupName={0}]", GroupName);
        }

    }

    public class FindByGroupQueryHandler
        : IQueryHandler<FindByGroupQuery, List<ActiveDirectoryUser>>
    {
        private readonly InfrastructureOptions _options;

        public FindByGroupQueryHandler(IOptions<InfrastructureOptions> options)
        {
            _options = options.Value;
        }

        public Task<List<ActiveDirectoryUser>> HandleAsync(FindByGroupQuery query)
        {

            var members = new List<ActiveDirectoryUser>();
            using (var context = new PrincipalContext(ContextType.Domain, _options.LdapIpAddress))
            {
                using (var group = GroupPrincipal.FindByIdentity(context, query.GroupName))
                {
                    if (group != null)
                    {
                        var users = group.GetMembers(false);
                        foreach (var principal in users)
                        {
                            //AuthenticablePrincipal user;
                            var member = new ActiveDirectoryUser();

                            if (principal.GetType() == typeof(UserPrincipal))
                            {
                                var user = (UserPrincipal)principal;
                                member = new ActiveDirectoryUser()
                                {
                                    DisplayName = user.DisplayName,
                                    FirstName = user.GivenName,
                                    LastName = user.Surname,
                                    Username = user.SamAccountName,
                                    EmailAddress = user.EmailAddress,
                                    PrincipleType = "User",
                                    TelephoneNumber = user.VoiceTelephoneNumber,
                                    Description = user.Description,
                                    DistinguishedName = user.DistinguishedName,
                                    Enabled = user.Enabled
                                };

                                if (user.GetUnderlyingObject() is DirectoryEntry de)
                                {
                                    if (de.Properties["thumbnailPhoto"].Value is byte[] data)
                                    {
                                        member.AvatarSource = $"data:image;base64,{Convert.ToBase64String(data)}";
                                    }
                                    else
                                    {
                                        member.AvatarSource = _options.DefaultUserAvatarImagePath;
                                    }
                                    member.Department = de.Properties["department"].Value?.ToString();
                                    member.Title = de.Properties["title"].Value?.ToString();
                                    member.TelephoneNumberExtension = de.Properties["ipPhone "].Value?.ToString();
                                }


                            }

                            if (principal.GetType() == typeof(ComputerPrincipal))
                            {
                                var user = (ComputerPrincipal)principal;
                                member = new ActiveDirectoryUser()
                                {
                                    DisplayName = user.DisplayName,
                                    FirstName = user.Name,
                                    LastName = string.Empty,
                                    Username = user.SamAccountName,
                                    EmailAddress = string.Empty,
                                    PrincipleType = "Computer"
                                };
                            }

                            members.Add(member);
                        }
                    }
                }
            }

            return Task.FromResult(members);
        }

    }
}