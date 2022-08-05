using MEI.SPDocuments.TypeCodes;

namespace MEI.SPDocuments.Security
{
    /// <summary>
    ///     Represents user privileges for a specific document type.
    /// </summary>
    public class DocumentPrivileges
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="DocumentPrivileges" /> class.
        /// </summary>
        /// <param name="documentType">Type of the document.</param>
        public DocumentPrivileges(SPDocumentType documentType)
        {
            DocumentType = documentType;
        }

        /// <summary>
        ///     Gets or sets the type of the document to which the user privileges pertain.
        /// </summary>
        public SPDocumentType DocumentType { get; }

        /// <summary>
        ///     Gets or sets the user privileges of the document type.
        /// </summary>
        public SPDocumentPrivileges Privileges { get; set; }

        /// <summary>
        ///     Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        ///     A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("[DocumentType={0}, Privileges={1}]", DocumentType.ToString(), Privileges.ToAbbreviatedText());
        }
    }
}
