using System.Collections.Generic;
using System.Linq;

using MEI.SPDocuments.ActiveDirectory;
using MEI.SPDocuments.Data;
using MEI.SPDocuments.TypeCodes;

namespace MEI.SPDocuments.Security
{
    /// <summary>
    ///     Provides fro privilege checks on document types.
    /// </summary>
    public interface IDocumentAccessControl
    {
        /// <summary>
        ///     Gets the domain groups for the user.
        /// </summary>
        IList<string> GetDomainGroupsForUser(string userName);

        /// <summary>
        ///     Gets the user's document privileges.
        /// </summary>
        IList<DocumentPrivileges> GetUserPrivileges(string userName);

        /// <summary>
        ///     Determines whether the user has permission for the specified <paramref name="privilege" /> on the specified
        ///     <paramref name="documentType" />.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="documentType">Type of the document.</param>
        /// <param name="privilege">The privilege.</param>
        /// <returns>
        ///     <c>true</c> if the user has the specified <paramref name="privilege" /> on the <paramref name="documentType" />
        ///     otherwise, <c>false</c>.
        /// </returns>
        bool HasPrivilege(string userName, SPDocumentType documentType, SPDocumentPrivileges privilege);
    }

    /// <summary>
    ///     Provides for privilege checks on document types.
    /// </summary>
    internal class DocumentAccessControl
        : IDocumentAccessControl
    {
        private readonly IActiveDirectoryControl _activeDirectoryControl;
        private readonly IRepository _repository;
        private readonly IDictionary<string, IList<DocumentPrivileges>> _usersPrivileges;
        private static object _lock = new object();

        /// <summary>
        ///     Initializes a new instance of the <see cref="DocumentAccessControl" /> class.
        /// </summary>
        /// <param name="activeDirectoryControl">The active directory control used to get the domain groups.</param>
        /// <param name="repository">The info helper.</param>
        public DocumentAccessControl(IActiveDirectoryControl activeDirectoryControl, IRepository repository)
        {
            _usersPrivileges = new Dictionary<string, IList<DocumentPrivileges>>();

            _activeDirectoryControl = activeDirectoryControl;
            _repository = repository;
        }

        /// <summary>
        ///     Gets the Active Directory domain groups for the user.
        /// </summary>
        public IList<string> GetDomainGroupsForUser(string userName)
        {
            return _activeDirectoryControl.GetDomainGroupsForUser(userName);
        }

        /// <summary>
        ///     Gets the user's document privileges.
        /// </summary>
        public IList<DocumentPrivileges> GetUserPrivileges(string userName)
        {
            lock (_lock)
            {
                if (!_usersPrivileges.ContainsKey(userName) || _usersPrivileges[userName] == null)
                {
                    IList<DocumentAccessInfo> documentAccessInfos = _repository.GetDocumentAccessInfos(SPDocumentType.None, GetDomainGroupsForUser(userName));

                    _usersPrivileges[userName] = GetPriorityPrivileges(documentAccessInfos);
                }   
            }

            return _usersPrivileges[userName];
        }

        /// <summary>
        ///     Determines whether the user has permission for the specified <paramref name="privilege" /> on the specified
        ///     <paramref name="documentType" />.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="documentType">Type of the document.</param>
        /// <param name="privilege">The privilege.</param>
        /// <returns>
        ///     <c>true</c> if the user has the specified <paramref name="privilege" /> on the <paramref name="documentType" />
        ///     otherwise, <c>false</c>.
        /// </returns>
        public bool HasPrivilege(string userName, SPDocumentType documentType, SPDocumentPrivileges privilege)
        {
            Preconditions.CheckEnum("documentType", documentType, SPDocumentType.None);
            Preconditions.CheckEnum("privilege", privilege, SPDocumentPrivileges.None);

            IList<DocumentPrivileges> privileges = GetUserPrivileges(userName);

            if (privileges?.Any(x => x.DocumentType == documentType) != true)
            {
                return false;
            }

            return privileges.First(x => x.DocumentType == documentType).Privileges.Has(privilege);
        }

        private IList<DocumentPrivileges> GetPriorityPrivileges(IList<DocumentAccessInfo> items)
        {
            var priorityPrivileges = new List<DocumentPrivileges>();

            foreach (DocumentAccessInfo dai in items)
            {
                int highestImportance = int.MaxValue;

                foreach (DocumentAccessInfo dai2 in items)
                {
                    if ((dai.DocumentType == dai2.DocumentType) && (dai.PrivilegeGroup == dai2.PrivilegeGroup))
                    {
                        if (dai2.PrivilegeImportance < highestImportance)
                        {
                            highestImportance = dai2.PrivilegeImportance;
                        }
                    }
                }

                foreach (DocumentAccessInfo dai3 in items)
                {
                    if ((dai3.DocumentType == dai.DocumentType) && (dai3.PrivilegeGroup == dai.PrivilegeGroup)
                                                                && (dai3.PrivilegeImportance == highestImportance))
                    {
                        if (priorityPrivileges.All(x => x.DocumentType != dai3.DocumentType))
                        {
                            priorityPrivileges.Add(new DocumentPrivileges(dai3.DocumentType));
                        }

                        DocumentPrivileges documentPrivileges = priorityPrivileges.First(x => x.DocumentType == dai3.DocumentType);
                        documentPrivileges.Privileges = documentPrivileges.Privileges.Add(dai3.Privilege);
                    }
                }
            }

            return priorityPrivileges;
        }
    }
}
