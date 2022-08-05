using MEI.SPDocuments.TypeCodes;

namespace MEI.SPDocuments
{
    public class DocumentTypeInfo
    {
        public DocumentTypeInfo(SPDocumentType documentType,
                                string name,
                                string displayName,
                                string acronym,
                                string folderName,
                                string prefixText,
                                string baseDocumentPath,
                                string baseWebPath,
                                bool isDisabled)
        {
            DocumentType = documentType;
            Name = name;
            DisplayName = displayName;
            Acronym = acronym;
            FolderName = folderName;
            PrefixText = prefixText;
            BaseDocumentPath = baseDocumentPath;
            BaseWebPath = baseWebPath;
            IsDisabled = isDisabled;
            SPFields = new SPFieldCollection();
        }

        public bool IsDisabled { get; }

        public string Name { get; }

        public string DisplayName { get; }

        public string Acronym { get; }

        public string FolderName { get; }

        public string PrefixText { get; }

        public SPDocumentType DocumentType { get; }

        public string BaseDocumentPath { get; }

        public string BaseWebPath { get; }

        public string ConvertedDocumentPath => string.Format("{0}{1}\\{1}ConvertedDocs\\", BaseDocumentPath, FolderName);

        public string FailedDocumentPath => string.Format("{0}{1}\\{1}FailedUploadDocs", BaseDocumentPath, FolderName);

        public string MergedDocumentPath => string.Format("{0}{1}\\{1}MergedDocs", BaseDocumentPath, FolderName);

        public string ArchivedDocumentPath => string.Format("{0}{1}\\{1}ArchivedDocs", BaseDocumentPath, FolderName);

        public string WebDocumentPath => string.Format("{0}{1}\\", BaseWebPath, FolderName);

        public SPFieldCollection SPFields { get; }
    }
}
