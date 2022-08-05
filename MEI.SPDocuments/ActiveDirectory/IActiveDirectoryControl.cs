using System;
using System.Collections.Generic;
using System.DirectoryServices;

using Microsoft.Extensions.Options;

namespace MEI.SPDocuments.ActiveDirectory
{
    /// <summary>
    ///     Provides active directory support.
    /// </summary>
    public interface IActiveDirectoryControl
        : IDisposable
    {
        /// <summary>
        ///     Gets the domain groups for the user.
        /// </summary>
        IList<string> GetDomainGroupsForUser(string userName);
    }

    /// <summary>
    ///     Provides Active Directory support.
    /// </summary>
    internal class ActiveDirectoryControl
        : IActiveDirectoryControl
    {
        private readonly IDictionary<string, IList<string>> _domainGroupsForUsers;
        private readonly IList<DirectorySearcher> _directorySearchers;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ActiveDirectoryControl" /> class.
        /// </summary>
        public ActiveDirectoryControl(IOptions<SPDocumentsOptions> options)
        {
            _domainGroupsForUsers = new Dictionary<string, IList<string>>();
            _directorySearchers = new List<DirectorySearcher>();

            foreach (string item in options.Value.LdapPaths)
            {
                var searcher = new DirectorySearcher(new DirectoryEntry(item,
                    options.Value.CredentialUserName,
                    options.Value.CredentialPassword));
                searcher.PropertiesToLoad.Add("memberOf");

                _directorySearchers.Add(searcher);
            }
        }

        /// <summary>
        ///     Gets the domain groups for the user.
        /// </summary>
        public IList<string> GetDomainGroupsForUser(string userName)
        {
            if (!_domainGroupsForUsers.ContainsKey(userName) || (_domainGroupsForUsers[userName].Count <= 0))
            {
                _domainGroupsForUsers.Add(userName, GetGroups(userName));
            }

            return _domainGroupsForUsers[userName];
        }

        private void SetUserNameFilter(string userName)
        {
            string tempUserName = userName.Contains("\\") ? userName.Substring(userName.IndexOf("\\") + 1) : userName;

            foreach (DirectorySearcher item in _directorySearchers)
            {
                item.Filter = string.Format("sAMAccountName={0}", tempUserName);
            }
        }

        private IList<string> GetGroups(string userName)
        {
            Preconditions.CheckNotNullOrEmpty("userName", userName);

            SetUserNameFilter(userName);

            var groups = new List<string>();

            foreach (DirectorySearcher searcher in _directorySearchers)
            {
                SearchResult result = searcher.FindOne();

                List<string> memberOfValues = FindOnePropertiesByName(result, "memberOf");

                if (memberOfValues == null)
                {
                    continue;
                }

                foreach (string value in memberOfValues)
                {
                    int equalsIndex = Convert.ToInt32(value.IndexOf("=", 1).ToString());
                    int commaIndex = Convert.ToInt32(value.IndexOf(",", 1).ToString());

                    if (equalsIndex == -1)
                    {
                        return null;
                    }

                    string extractedMemberOf = value.Substring(equalsIndex + 1, commaIndex - equalsIndex - 1);

                    if (!groups.Contains(extractedMemberOf))
                    {
                        groups.Add(extractedMemberOf);
                    }
                }
            }

            groups.Add(userName);

            return groups;
        }

        private List<string> FindOnePropertiesByName(SearchResult result, string propertyName)
        {
            var properties = new List<string>();

            if (result == null)
            {
                return new List<string>();
            }

            if (!result.Properties.Contains(propertyName))
            {
                throw new ArgumentException(string.Format(Resources.Default.The_properties_of_this_result_do_not_contain_this_property_name___0_,
                    propertyName));
            }

            ResultPropertyValueCollection values = result.Properties[propertyName];

            foreach (object value in values)
            {
                properties.Add(value.ToString());
            }

            return properties;
        }

        public void Dispose()
        {
            if (_directorySearchers != null)
            {
                foreach (DirectorySearcher directorySearcher in _directorySearchers)
                {
                    directorySearcher.Dispose();
                }
            }
        }
    }
}
