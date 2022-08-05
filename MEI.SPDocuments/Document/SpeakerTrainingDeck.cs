using System.Collections;
using System.Collections.Generic;

using MEI.SPDocuments.Data;
using MEI.SPDocuments.TypeCodes;

namespace MEI.SPDocuments.Document
{
    [DocumentInfo(SPDocumentType.SpeakerTrainingDeck, "SpeakerTrainingDeck", "SPTD")]
    public class SpeakerTrainingDeck
        : SPDocumentBase, ISearchYear
    {
        internal SpeakerTrainingDeck(IRepository repository, IDbUtilities dbUtilities, DocumentTypeInfo documentTypeInfo)
            : base(repository, dbUtilities, documentTypeInfo)
        { }

        public SpeakerTrainingDeck WithValues(string testColumn)
        {
            TestColumn = testColumn;

            return this;
        }

        [SPFieldInfo(SPFieldNames.DocumentYear, "DocumentYear", SPFieldType.Text, 1)]
        public override DocumentYear DocumentYear => DocumentYear.Year2014;

        public override string SearchFieldValue => TestColumn;

        [SPFieldInfo(SPFieldNames.TestColumn, "TestColumn", SPFieldType.Text, "TestColumn", 0)]
        public string TestColumn { get; private set; }

        public override string FileName => MakeFileName(TestColumn, DocumentYear.Year2014.ToDisplayNameLong());

        public override bool IsValid
        {
            get
            {
                bool baseValid = base.IsValid;

                return baseValid;
            }
        }

        public override string UniqueIdentifiers => "TestColumn;DocumentYear";

        public override string UniqueValues => string.Format("{0};{1}", TestColumn, DocumentYear.Year2014.ToProgramIdYear());

        public override IList<string> AllowedFileTypes =>
            new List<string>
            {
                "zip"
            };

        public ISearchExpressionGroup GetSearchExpressionGroupByYear(DocumentYear year)
        {
            if (year == DocumentYear.Undefined)
            {
                return new SearchExpressionGroup(this);
            }

            return new SearchExpressionGroup(this, SPFieldNames.DocumentYear, CamlComparison.Equal, year.ToString());
        }

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

            TestColumn = objects[0].ToString();
            Contents = (byte[])objects[1];
            FileExtension = objects[2].ToString();
            Company = (Company)objects[3];

            return IsValid;
        }

        public override bool AbstractSetup(Hashtable values)
        {
            if (values.ContainsKey(SPFields[SPFieldNames.TestColumn].InternalName))
            {
                TestColumn = (string)values[SPFields[SPFieldNames.TestColumn].InternalName];
            }

            return true;
        }

        public override IDictionary<string, string> GetUserFieldValues()
        {
            return new Dictionary<string, string>
                   {
                       { SPFields[SPFieldNames.TestColumn].InternalName, TestColumn },
                       { SPFields[SPFieldNames.DocumentYear].InternalName, DocumentYear.Year2014.ToDisplayNameLong() }
                   };
        }

        public override string[] ParseFileName(string fileNameToParse)
        {
            string[] fileNameParts = base.ParseFileName(fileNameToParse);

            TestColumn = fileNameParts[1];

            if (DocumentYear == DocumentYear.Undefined)
            {
                ThrowFileNameExceptionInvalidType(fileNameToParse, SPFieldNames.DocumentYear, "DocumentYear");
            }

            return fileNameParts;
        }
    }
}
