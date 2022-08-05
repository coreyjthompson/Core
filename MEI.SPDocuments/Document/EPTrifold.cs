using System;
using System.Collections;
using System.Collections.Generic;

using MEI.SPDocuments.Data;
using MEI.SPDocuments.TypeCodes;

namespace MEI.SPDocuments.Document
{
    [DocumentInfo(SPDocumentType.EPTrifold, "EPTrifold", "EPTF")]
    public class EPTrifold
        : SPDocumentBase
    {
        internal EPTrifold(IRepository repository, IDbUtilities dbUtilities, DocumentTypeInfo documentTypeInfo)
            : base(repository, dbUtilities, documentTypeInfo)
        { }

        public EPTrifold WithValues(int? trifoldId, EPassStatus statusTypeCode)
        {
            TrifoldId = trifoldId;
            Status = statusTypeCode;

            return this;
        }

        public override DocumentYear DocumentYear => DocumentYear.Undefined;

        [SPFieldInfo(SPFieldNames.TrifoldId, "TrifoldID", SPFieldType.Text, 0)]
        public int? TrifoldId { get; private set; }

        [SPFieldInfo(SPFieldNames.StatusCode, "StatusCode", SPFieldType.Text, 1)]
        public EPassStatus Status { get; private set; }

        public override string FileName => MakeFileName(TrifoldId, Status.ToDisplayNameShort());

        public override bool IsValid
        {
            get
            {
                bool baseValid = base.IsValid;

                if (!TrifoldId.HasValue)
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

        public override string UniqueIdentifiers => "TrifoldId;Status";

        public override string UniqueValues => string.Format("{0};{1}", TrifoldId, Status.ToDisplayNameLong());

        public override bool ValidateFields()
        {
            if (!IsValid)
            {
                return false;
            }

            if (!TrifoldId.HasValue)
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

            TrifoldId = Convert.ToInt32(objects[0]);
            Status = objects[1].ToString().ToEPassStatus();
            Contents = (byte[])objects[2];
            FileExtension = objects[3].ToString();
            Company = (Company)objects[4];

            return IsValid;
        }

        public override bool AbstractSetup(Hashtable values)
        {
            if (values.ContainsKey(SPFields[SPFieldNames.TrifoldId].InternalName))
            {
                TrifoldId = Convert.ToInt32(values[SPFields[SPFieldNames.TrifoldId].InternalName]);
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
                       { SPFields[SPFieldNames.TrifoldId].InternalName, TrifoldId.ToString() },
                       { SPFields[SPFieldNames.StatusCode].InternalName, Status.ToDisplayNameShort() }
                   };
        }

        public override string[] ParseFileName(string fileNameToParse)
        {
            string[] fileNameParts = base.ParseFileName(fileNameToParse);

            if (!int.TryParse(fileNameParts[1], out int tempTrifoldId))
            {
                ThrowFileNameExceptionInvalidType(fileNameToParse, SPFieldNames.TrifoldId, "Integer");
            }

            TrifoldId = tempTrifoldId;

            Status = fileNameParts[2].ToEPassStatus();

            if (Status == EPassStatus.Undefined)
            {
                ThrowFileNameExceptionInvalidType(fileNameToParse, SPFieldNames.StatusCode, "EPassStatusCode");
            }

            return fileNameParts;
        }
    }
}
