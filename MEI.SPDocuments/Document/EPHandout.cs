using System;
using System.Collections;
using System.Collections.Generic;

using MEI.SPDocuments.Data;
using MEI.SPDocuments.TypeCodes;

namespace MEI.SPDocuments.Document
{
    [DocumentInfo(SPDocumentType.EPHandout, "EPHandout", "EPHO")]
    public class EPHandout
        : SPDocumentBase
    {
        internal EPHandout(IRepository repository, IDbUtilities dbUtilities, DocumentTypeInfo documentTypeInfo)
            : base(repository, dbUtilities, documentTypeInfo)
        { }

        public EPHandout WithValues(int? handoutId, EPassStatus statusTypeCode)
        {
            HandoutId = handoutId;
            Status = statusTypeCode;

            return this;
        }

        public override DocumentYear DocumentYear => DocumentYear.Undefined;

        [SPFieldInfo(SPFieldNames.HandoutId, "HandoutID", SPFieldType.Text, 0)]
        public int? HandoutId { get; private set; }

        [SPFieldInfo(SPFieldNames.StatusCode, "StatusCode", SPFieldType.Text, 1)]
        public EPassStatus Status { get; private set; }

        public override string FileName => MakeFileName(HandoutId, Status.ToDisplayNameShort());

        public override bool IsValid
        {
            get
            {
                bool baseValid = base.IsValid;

                if (!HandoutId.HasValue)
                {
                    return false;
                }

                if (Status == EPassStatus.Undefined)
                {
                    return false;
                }

                return baseValid;
            }
        }

        public override IList<string> AllowedFileTypes =>
            new List<string>
            {
                "pdf",
                "ppt",
                "pptx",
                "doc",
                "docx"
            };

        public override string UniqueIdentifiers => "HandoutId;Status";

        public override string UniqueValues => string.Format("{0};{1}", HandoutId, Status.ToDisplayNameLong());

        public override bool ValidateFields()
        {
            if (!IsValid)
            {
                return false;
            }

            if (!HandoutId.HasValue)
            {
                return false;
            }

            if (Status == EPassStatus.Undefined)
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

            HandoutId = Convert.ToInt32(objects[0]);
            Status = objects[1].ToString().ToEPassStatus();
            Contents = (byte[])objects[2];
            FileExtension = objects[3].ToString();
            Company = (Company)objects[4];

            return IsValid;
        }

        public override bool AbstractSetup(Hashtable values)
        {
            if (values.ContainsKey(SPFields[SPFieldNames.HandoutId].InternalName))
            {
                HandoutId = Convert.ToInt32(values[SPFields[SPFieldNames.HandoutId].InternalName]);
            }

            if (values.ContainsKey(SPFields[SPFieldNames.StatusCode].InternalName))
            {
                Status = ((string)values[SPFields[SPFieldNames.StatusCode].InternalName]).ToEPassStatus();
            }

            return true;
        }

        public override IDictionary<string, string> GetUserFieldValues()
        {
            return new Dictionary<string, string>
                   {
                       { SPFields[SPFieldNames.HandoutId].InternalName, HandoutId.ToString() },
                       { SPFields[SPFieldNames.StatusCode].InternalName, Status.ToDisplayNameShort() }
                   };
        }

        public override string[] ParseFileName(string fileNameToParse)
        {
            string[] fileNameParts = base.ParseFileName(fileNameToParse);

            if (!int.TryParse(fileNameParts[1], out int tempHandoutId))
            {
                ThrowFileNameExceptionInvalidType(fileNameToParse, SPFieldNames.HandoutId, "Integer");
            }

            HandoutId = tempHandoutId;

            Status = fileNameParts[2].ToEPassStatus();
            if (Status == EPassStatus.Undefined)
            {
                ThrowFileNameExceptionInvalidType(fileNameToParse, SPFieldNames.StatusCode, "EPassStatusCode");
            }

            return fileNameParts;
        }
    }
}
