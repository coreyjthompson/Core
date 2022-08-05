using MEI.SPDocuments.TypeCodes;

namespace MEI.SPDocuments.Security
{
    /// <summary>
    ///     Represents a privilege on a specific documentType.
    /// </summary>
    public class DocumentAccessInfo
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="DocumentAccessInfo" /> class.
        /// </summary>
        /// <param name="documentType">Type of the document.</param>
        /// <param name="documentPrivilege">The privilege.</param>
        /// <param name="documentPrivilegeGroup">The group that the privilege belongs to.</param>
        /// <param name="documentPrivilegeImportance">The importance scale of the privilege within its respective group.</param>
        public DocumentAccessInfo(SPDocumentType documentType,
                                    SPDocumentPrivileges documentPrivilege,
                                    string documentPrivilegeGroup,
                                    int documentPrivilegeImportance)
        {
            Preconditions.CheckEnum("documentPrivilege", documentPrivilege, SPDocumentPrivileges.None);
            Preconditions.CheckEnum("documentType", documentType, SPDocumentType.None);
            Preconditions.CheckNotNullOrEmpty("documentPrivilegeGroup", documentPrivilegeGroup);

            DocumentType = documentType;
            Privilege = documentPrivilege;
            PrivilegeGroup = documentPrivilegeGroup;
            PrivilegeImportance = documentPrivilegeImportance;
        }

        /// <summary>
        ///     Gets the type of the document.
        /// </summary>
        internal SPDocumentType DocumentType { get; }

        /// <summary>
        ///     Gets the privilege.
        /// </summary>
        internal SPDocumentPrivileges Privilege { get; }

        /// <summary>
        ///     Gets the group that the privilege belongs to.
        /// </summary>
        internal string PrivilegeGroup { get; }

        /// <summary>
        ///     Gets the importance scale of the privilege within its respective group.
        /// </summary>
        internal int PrivilegeImportance { get; }

        /// <summary>
        ///     Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        ///     A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("[DocumentType={0}, Privilege={1}, PrivilegeGroup={2}, PrivilegeImportance={3}]",
                DocumentType.ToString(),
                Privilege.ToNameText(),
                PrivilegeGroup,
                PrivilegeImportance);
        }
    }
}
