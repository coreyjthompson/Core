using System;

using MEI.SPDocuments.TypeCodes;

namespace MEI.SPDocuments
{
    [AttributeUsage(AttributeTargets.Class)]
    internal class DocumentInfoAttribute
        : Attribute
    {
        public DocumentInfoAttribute(SPDocumentType documentType,
                                     string name,
                                     string acronym,
                                     string prefixText,
                                     string displayName,
                                     string folderName)
        {
            DocumentType = documentType;
            Name = name;
            Acronym = acronym;
            PrefixText = prefixText;
            DisplayName = displayName;
            FolderName = folderName;
        }

        public DocumentInfoAttribute(SPDocumentType documentType, string name)
            : this(documentType, name, name, name, name, name)
        { }

        public DocumentInfoAttribute(SPDocumentType documentType, string name, string acronym)
            : this(documentType, name, acronym, acronym, name, name)
        { }

        public DocumentInfoAttribute(SPDocumentType documentType, string name, string acronym, string prefixText)
            : this(documentType, name, acronym, prefixText, name, name)
        { }

        public DocumentInfoAttribute(SPDocumentType documentType, string name, string acronym, string prefixText, string displayName)
            : this(documentType, name, acronym, prefixText, displayName, name)
        { }

        public SPDocumentType DocumentType { get; }

        public string Name { get; }

        public string Acronym { get; }

        public string DisplayName { get; }

        public string FolderName { get; }

        public string PrefixText { get; }
    }
}
