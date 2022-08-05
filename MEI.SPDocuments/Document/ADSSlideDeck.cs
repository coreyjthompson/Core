using System;
using System.Collections;
using System.Collections.Generic;

using MEI.SPDocuments.Data;
using MEI.SPDocuments.TypeCodes;

namespace MEI.SPDocuments.Document
{
    [DocumentInfo(SPDocumentType.ADSSlideDeck, "ADSSlideDeck", "ADSSD", "ADSSD", "ADS SlideDeck")]
    public class ADSSlideDeck
        : SPDocumentBase
    {
        internal ADSSlideDeck(IRepository repository, IDbUtilities dbUtilities, DocumentTypeInfo documentTypeInfo)
            : base(repository, dbUtilities, documentTypeInfo)
        { }

        public ADSSlideDeck WithValues(string versionNumber,
                               long? documentSearchProductId,
                               string uploadUserName,
                               string slideDeckTitle,
                               string keywords)
        {
            DocumentSearchProductId = documentSearchProductId;
            Keywords = keywords;
            SlideDeckTitle = slideDeckTitle;
            UploadUserName = uploadUserName;
            VersionNumber = versionNumber;

            return this;
        }

        public override DocumentYear DocumentYear => DocumentYear.Undefined;

        public override string ParsableFileName => MakeFileName(VersionNumber, SlideDeckTitle, UploadUserName, "", DocumentSearchProductId);

        public override IList<string> AllowedFileTypes =>
            new List<string>
            {
                "pdf",
                "pptx",
                "ppt"
            };

        [SPFieldInfo(SPFieldNames.DocumentSearchProductId, "DocumentSearchProductId", SPFieldType.Number, 2)]
        public long? DocumentSearchProductId { get; private set; }

        [SPFieldInfo(SPFieldNames.Keywords, "Keywords", SPFieldType.Text, 3)]
        public string Keywords { get; private set; }

        [SPFieldInfo(SPFieldNames.SlideDeckTitle, "SlideDeckTitle", SPFieldType.Text, 0)]
        public string SlideDeckTitle { get; private set; }

        [SPFieldInfo(SPFieldNames.UploadUserName, "UploadUserName", SPFieldType.Text, 4)]
        public string UploadUserName { get; private set; }

        [SPFieldInfo(SPFieldNames.VersionNumber, "VersionNumber", SPFieldType.Text, 1)]
        public string VersionNumber { get; private set; }

        public override string FileName => MakeFileName(SlideDeckTitle, VersionNumber);

        public override bool IsValid
        {
            get
            {
                bool baseValid = base.IsValid;

                if (string.IsNullOrEmpty(VersionNumber))
                {
                    return false;
                }

                if (string.IsNullOrEmpty(SlideDeckTitle))
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

        public override string UniqueIdentifiers => "SlideDeckTitle;VersionNumber";

        public override string UniqueValues => string.Format("{0};{1}", SlideDeckTitle, VersionNumber);

        public override bool ValidateFields()
        {
            if (!IsValid)
            {
                return false;
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

            VersionNumber = objects[0].ToString();
            DocumentSearchProductId = Convert.ToInt64(objects[1]);
            UploadUserName = objects[2].ToString();
            SlideDeckTitle = objects[3].ToString();
            Keywords = objects[4].ToString();
            Contents = (byte[])objects[5];
            FileExtension = objects[6].ToString();
            Company = (Company)objects[7];

            return IsValid;
        }

        public override bool AbstractSetup(Hashtable values)
        {
            if (values.ContainsKey(SPFields[SPFieldNames.VersionNumber].InternalName))
            {
                VersionNumber = (string)values[SPFields[SPFieldNames.VersionNumber].InternalName];
            }

            if (values.ContainsKey(SPFields[SPFieldNames.UploadUserName].InternalName))
            {
                UploadUserName = (string)values[SPFields[SPFieldNames.UploadUserName].InternalName];
            }

            if (values.ContainsKey(SPFields[SPFieldNames.SlideDeckTitle].InternalName))
            {
                SlideDeckTitle = (string)values[SPFields[SPFieldNames.SlideDeckTitle].InternalName];
            }

            if (values.ContainsKey(SPFields[SPFieldNames.Keywords].InternalName))
            {
                Keywords = (string)values[SPFields[SPFieldNames.Keywords].InternalName];
            }

            if (values.ContainsKey(SPFields[SPFieldNames.DocumentSearchProductId].InternalName))
            {
                DocumentSearchProductId =
                    Convert.ToInt32(values[SPFields[SPFieldNames.DocumentSearchProductId].InternalName]);
            }

            return true;
        }

        public override IDictionary<string, string> GetUserFieldValues()
        {
            return new Dictionary<string, string>
                   {
                       { SPFields[SPFieldNames.DocumentSearchProductId].InternalName, DocumentSearchProductId.ToString() },
                       { SPFields[SPFieldNames.UploadUserName].InternalName, UploadUserName },
                       { SPFields[SPFieldNames.SlideDeckTitle].InternalName, SlideDeckTitle },
                       { SPFields[SPFieldNames.VersionNumber].InternalName, VersionNumber },
                       { SPFields[SPFieldNames.Keywords].InternalName, Keywords }
                   };
        }

        public override string[] ParseFileName(string fileNameToParse)
        {
            string[] fileNameParts = base.ParseFileName(fileNameToParse);

            if (string.IsNullOrEmpty(fileNameParts[1]))
            {
                ThrowFileNameExceptionInvalidType(fileNameParts[1], SPFieldNames.VersionNumber, "String");
            }

            VersionNumber = fileNameParts[1];

            if (string.IsNullOrEmpty(fileNameParts[2]))
            {
                ThrowFileNameExceptionInvalidType(fileNameParts[2], SPFieldNames.SlideDeckTitle, "String");
            }

            SlideDeckTitle = fileNameParts[2];

            if (string.IsNullOrEmpty(fileNameParts[3]))
            {
                ThrowFileNameExceptionInvalidType(fileNameParts[3], SPFieldNames.UploadUserName, "String");
            }

            UploadUserName = fileNameParts[3];

            Keywords = fileNameParts[4];

            DocumentSearchProductId = Convert.ToInt64(fileNameParts[5]);

            return fileNameParts;
        }
    }
}
