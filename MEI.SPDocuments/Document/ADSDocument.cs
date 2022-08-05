using System;
using System.Collections;
using System.Collections.Generic;

using MEI.SPDocuments.Data;
using MEI.SPDocuments.TypeCodes;

namespace MEI.SPDocuments.Document
{
    [DocumentInfo(SPDocumentType.ADSDocument, "ADSDocument", "ADSD", "ADSD", "ADS Document")]
    public class ADSDocument
        : SPDocumentBase
    {
        internal ADSDocument(IRepository repository, IDbUtilities dbUtilities, DocumentTypeInfo documentTypeInfo)
            : base(repository, dbUtilities, documentTypeInfo)
        { }

        public ADSDocument WithValues(long? documentSearchDocumentTypeId, string uploadUserName, string documentTitle, string keywords)
        {
            DocumentSearchDocumentTypeId = documentSearchDocumentTypeId;
            DocumentTitle = documentTitle;
            Keywords = keywords;
            UploadUserName = uploadUserName;

            return this;
        }

        public override DocumentYear DocumentYear => DocumentYear.Undefined;

        public override string ParsableFileName => MakeFileName(DocumentTitle, DocumentSearchDocumentTypeId, UploadUserName);

        [SPFieldInfo(SPFieldNames.DocumentSearchDocumentTypeId, "DocumentSearchDocumentTypeId", SPFieldType.Text, 1)]
        public long? DocumentSearchDocumentTypeId { get; private set; }

        [SPFieldInfo(SPFieldNames.DocumentTitle, "DocumentTitle", SPFieldType.Text, 0)]
        public string DocumentTitle { get; private set; }

        [SPFieldInfo(SPFieldNames.Keywords, "Keywords", SPFieldType.Text, 2)]
        public string Keywords { get; private set; }

        [SPFieldInfo(SPFieldNames.UploadUserName, "UploadUserName", SPFieldType.Text, 3)]
        public string UploadUserName { get; private set; }

        public override string FileName => MakeFileName(DocumentTitle, DocumentSearchDocumentTypeId);

        public override bool IsValid
        {
            get
            {
                bool baseValid = base.IsValid;

                if (string.IsNullOrEmpty(DocumentTitle))
                {
                    return false;
                }

                if (!DocumentSearchDocumentTypeId.HasValue)
                {
                    return false;
                }

                if (string.IsNullOrEmpty(UploadUserName))
                {
                    return false;
                }

                return baseValid;
            }
        }

        public override string UniqueIdentifiers => "DocumentTitle;DocumentSearchDocumentTypeId";

        public override string UniqueValues => string.Format("{0};{1}", DocumentTitle, DocumentSearchDocumentTypeId);

        public override IList<string> AllowedFileTypes =>
            new List<string>
            {
                "pdf",
                "xlsx",
                "docx",
                "pptx"
            };

        public override bool ValidateFields()
        {
            if (!IsValid)
            {
                return false;
            }

            if (DocumentSearchDocumentTypeId != null && Repository
                    .GetDocumentSearchDocumentTypeIdsByDocumentSearchDocumentTypeId(Company,
                        DocumentYear.Year2005,
                        DocumentSearchDocumentTypeId.Value).Rows.Count <= 0)
            {
                ThrowFileNameExceptionNoDBMatch(SPFieldNames.DocumentSearchDocumentTypeId, DocumentSearchDocumentTypeId.Value.ToString());
            }

            return true;
        }

        public override bool Setup(object[] objects)
        {
            int userFieldCount = GetUserFieldCount();

            //Add three to userFieldCount for the contents, fileExtension, and company
            userFieldCount += 3;

            if (objects.Length != userFieldCount)
            {
                return false;
            }

            DocumentSearchDocumentTypeId = Convert.ToInt64(objects[0]);
            UploadUserName = objects[1].ToString();
            DocumentTitle = objects[2].ToString();
            Keywords = objects[3].ToString();
            Contents = (byte[])objects[4];
            FileExtension = objects[5].ToString();
            Company = (Company)objects[6];

            return IsValid;
        }

        public override bool AbstractSetup(Hashtable values)
        {
            if (values.ContainsKey(SPFields[SPFieldNames.UploadUserName].InternalName))
            {
                UploadUserName = (string)values[SPFields[SPFieldNames.UploadUserName].InternalName];
            }

            if (values.ContainsKey(SPFields[SPFieldNames.Keywords].InternalName))
            {
                Keywords = (string)values[SPFields[SPFieldNames.Keywords].InternalName];
            }

            if (values.ContainsKey(SPFields[SPFieldNames.DocumentTitle].InternalName))
            {
                DocumentTitle = (string)values[SPFields[SPFieldNames.DocumentTitle].InternalName];
            }

            if (values.ContainsKey(SPFields[SPFieldNames.DocumentSearchDocumentTypeId].InternalName))
            {
                DocumentSearchDocumentTypeId =
                    Convert.ToInt32(values[SPFields[SPFieldNames.DocumentSearchDocumentTypeId].InternalName]);
            }

            return true;
        }

        public override IDictionary<string, string> GetUserFieldValues()
        {
            return new Dictionary<string, string>
                   {
                       { SPFields[SPFieldNames.DocumentSearchDocumentTypeId].InternalName, DocumentSearchDocumentTypeId.ToString() },
                       { SPFields[SPFieldNames.UploadUserName].InternalName, UploadUserName },
                       { SPFields[SPFieldNames.DocumentTitle].InternalName, DocumentTitle },
                       { SPFields[SPFieldNames.Keywords].InternalName, Keywords }
                   };
        }

        public override string[] ParseFileName(string fileNameToParse)
        {
            string[] fileNameParts = base.ParseFileName(fileNameToParse);

            DocumentTitle = fileNameParts[1];

            if (!long.TryParse(fileNameParts[2], out long tempDocumentSearchDocumentTypeId))
            {
                ThrowFileNameExceptionInvalidType(fileNameToParse, SPFieldNames.DocumentSearchDocumentTypeId, "Long");
            }

            DocumentSearchDocumentTypeId = tempDocumentSearchDocumentTypeId;

            if (string.IsNullOrEmpty(fileNameParts[3]))
            {
                ThrowFileNameExceptionInvalidType(fileNameToParse, SPFieldNames.UploadUserName, "String");
            }

            UploadUserName = fileNameParts[3];

            Keywords = fileNameParts[4];

            return fileNameParts;
        }
    }
}
