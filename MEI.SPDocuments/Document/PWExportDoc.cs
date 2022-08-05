using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

using MEI.SPDocuments.Data;
using MEI.SPDocuments.TypeCodes;

namespace MEI.SPDocuments.Document
{
    [DocumentInfo(SPDocumentType.PED, "PWExportDoc", "PED")]
    public class PWExportDoc
        : SPDocumentBase
    {
        internal PWExportDoc(IRepository repository, IDbUtilities dbUtilities, DocumentTypeInfo documentTypeInfo)
            : base(repository, dbUtilities, documentTypeInfo)
        { }

        public PWExportDoc WithValues(DateTime dateTime)
        {
            MyDateTime = dateTime;

            return this;
        }

        public override DocumentYear DocumentYear => MyDateTime.Year.ToString().ToDocumentYear();

        [SPFieldInfo(SPFieldNames.DateTime, "DateTime", SPFieldType.Text, 0)]
        public DateTime MyDateTime { get; private set; }

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
                "txt"
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

            MyDateTime = Convert.ToDateTime(objects[0]);
            Contents = (byte[])objects[1];
            FileExtension = objects[2].ToString();
            Company = (Company)objects[3];

            return IsValid;
        }

        public override bool AbstractSetup(Hashtable values)
        {
            if (values.ContainsKey(SPFields[SPFieldNames.DateTime].InternalName))
            {
                MyDateTime = Convert.ToDateTime(values[SPFields[SPFieldNames.DateTime].InternalName]);
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

            MyDateTime = tempDateTime;

            return fileNameParts;
        }
    }
}
