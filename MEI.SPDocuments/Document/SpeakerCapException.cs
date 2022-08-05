using System;
using System.Collections;
using System.Collections.Generic;

using MEI.SPDocuments.Data;
using MEI.SPDocuments.TypeCodes;

namespace MEI.SPDocuments.Document
{
    [DocumentInfo(SPDocumentType.SpeakerCapException, "SpeakerCapException", "SCE", "SpeakerCapException", "Speaker Cap Exception")]
    public class SpeakerCapException
        : SPDocumentBase, ISearchSpeaker, ISearchYear
    {
        internal SpeakerCapException(IRepository repository, IDbUtilities dbUtilities, DocumentTypeInfo documentTypeInfo)
            : base(repository, dbUtilities, documentTypeInfo)
        { }

        public SpeakerCapException WithValues(int? speakerCounter, DocumentYear documentYear)
        {
            SpeakerCounter = speakerCounter;
            DocumentYear = documentYear;

            return this;
        }

        [SPFieldInfo(SPFieldNames.DocumentYear, "Year", SPFieldType.Text, 0)]
        public override DocumentYear DocumentYear { get; internal set; }

        [SPFieldInfo(SPFieldNames.SpeakerCounter, "SpeakerCounter", SPFieldType.Text, 1)]
        public int? SpeakerCounter { get; private set; }

        public override string FileName => MakeFileName(SpeakerCounter, DocumentYear.ToDisplayNameLong());

        public override string ParsableFileName => MakeFileName(SpeakerCounter, DocumentYear.ToDisplayNameLong());

        public override bool IsValid
        {
            get
            {
                bool baseValid = base.IsValid;

                if (!SpeakerCounter.HasValue)
                {
                    return false;
                }

                if (DocumentYear == DocumentYear.Undefined)
                {
                    return false;
                }

                return baseValid;
            }
        }

        public override string UniqueIdentifiers => "SpeakerCounter;Year";

        public override string UniqueValues => string.Format("{0};{1}", SpeakerCounter, DocumentYear.ToProgramIdYear());

        public ISearchExpressionGroup GetSearchExpressionGroupBySpeaker(Company company, DocumentYear year, int speakerCounter)
        {
            return new SearchExpressionGroup(this, SPFieldNames.SpeakerCounter, CamlComparison.Equal, speakerCounter);
        }

        public ISearchExpressionGroup GetSearchExpressionGroupByYear(DocumentYear year)
        {
            if (year == DocumentYear.Undefined)
            {
                return new SearchExpressionGroup(this);
            }

            return new SearchExpressionGroup(this, SPFieldNames.DocumentYear, CamlComparison.Equal, year.ToDisplayNameLong());
        }

        public override bool ValidateFields()
        {
            if (!IsValid)
            {
                return false;
            }

            if (SpeakerCounter != null && Repository.GetSpeakerCountersBySpeakerCounter(Company, DocumentYear, SpeakerCounter.Value).Rows.Count <= 0)
            {
                ThrowFileNameExceptionNoDBMatch(SPFieldNames.SpeakerCounter, SpeakerCounter.Value.ToString());
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

            SpeakerCounter = Convert.ToInt32(objects[1]);
            DocumentYear = objects[2].ToString().ToDocumentYear();
            Contents = (byte[])objects[3];
            FileExtension = objects[4].ToString();
            Company = (Company)objects[5];

            return IsValid;
        }

        public override bool AbstractSetup(Hashtable values)
        {
            if (values.ContainsKey(SPFields[SPFieldNames.SpeakerCounter].InternalName))
            {
                SpeakerCounter = Convert.ToInt32(values[SPFields[SPFieldNames.SpeakerCounter].InternalName]);
            }

            if (values.ContainsKey(SPFields[SPFieldNames.DocumentYear].InternalName))
            {
                DocumentYear = ((string)values[SPFields[SPFieldNames.DocumentYear].InternalName]).ToDocumentYear();
            }

            return IsValid;
        }

        public override IDictionary<string, string> GetUserFieldValues()
        {
            return new Dictionary<string, string>
                   {
                       { SPFields[SPFieldNames.SpeakerCounter].InternalName, SpeakerCounter.ToString() },
                       { SPFields[SPFieldNames.DocumentYear].InternalName, DocumentYear.ToDisplayNameLong() }
                   };
        }

        public override string[] ParseFileName(string fileNameToParse)
        {
            string[] fileNameParts = base.ParseFileName(fileNameToParse);

            if (!int.TryParse(fileNameParts[1], out int tempSpeakerCounter))
            {
                ThrowFileNameExceptionInvalidType(fileNameToParse, SPFieldNames.SpeakerCounter, "Integer");
            }

            SpeakerCounter = tempSpeakerCounter;

            string tempDocumentYear = fileNameParts[2];
            DocumentYear = tempDocumentYear.ToDocumentYear();

            return fileNameParts;
        }
    }
}
