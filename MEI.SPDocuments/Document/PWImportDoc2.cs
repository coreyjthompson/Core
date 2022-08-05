using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

using MEI.SPDocuments.Data;
using MEI.SPDocuments.TypeCodes;

namespace MEI.SPDocuments.Document
{
    [DocumentInfo(SPDocumentType.PID2, "PWImportDoc2", "PID2")]
    public class PWImportDoc2
        : SPDocumentBase
    {
        private DateTime _dateTime;

        internal PWImportDoc2(IRepository repository, IDbUtilities dbUtilities, DocumentTypeInfo documentTypeInfo)
            : base(repository, dbUtilities, documentTypeInfo)
        { }

        public PWImportDoc2 WithValues(DateTime dateTime)
        {
            _dateTime = dateTime;

            return this;
        }

        public override DocumentYear DocumentYear => MyDateTime.Year.ToString().ToDocumentYear();

        [SPFieldInfo(SPFieldNames.DateTime, "DateTime", SPFieldType.Text, 0)]
        public DateTime MyDateTime => _dateTime;

        public override string FileName => MakeFileName(MyDateTime.ToString("yyyyMMddHHmmss"));

        public override bool IsValid
        {
            get
            {
                bool baseValid = base.IsValid;

                return baseValid;
            }
        }

        public override IList<string> AllowedFileTypes =>
            new List<string>
            {
                "pdf",
                "doc",
                "docx",
                "txt",
                "csv",
                "xls",
                "xlsx"
            };

        public override string UniqueIdentifiers => "DateTime";

        public override string UniqueValues => MyDateTime.ToString("yyyyMMddHHmmss");

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

            _dateTime = Convert.ToDateTime(objects[0]);
            Contents = (byte[])objects[1];
            FileExtension = objects[2].ToString();
            Company = (Company)objects[3];

            return IsValid;
        }

        public override bool AbstractSetup(Hashtable values)
        {
            if (values.ContainsKey(SPFields[SPFieldNames.DateTime].InternalName))
            {
                DateTime.TryParseExact(values[SPFields[SPFieldNames.DateTime].InternalName].ToString(),
                    "yyyyMMddHHmmss",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out _dateTime);
            }

            return true;
        }

        public override IDictionary<string, string> GetUserFieldValues()
        {
            return new Dictionary<string, string>
                   {
                       { SPFields[SPFieldNames.DateTime].InternalName, MyDateTime.ToString("yyyyMMddHHmmss") }
                   };
        }

        public override string[] ParseFileName(string fileNameToParse)
        {
            string[] fileNameParts = base.ParseFileName(fileNameToParse);

            if (!DateTime.TryParseExact(fileNameParts[1],
                "yyyyMMddHHmmss",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime tempDateTime))
            {
                ThrowFileNameExceptionInvalidType(fileNameToParse, SPFieldNames.DateTime, "DateTime");
            }

            _dateTime = tempDateTime;

            return fileNameParts;
        }
    }
}
