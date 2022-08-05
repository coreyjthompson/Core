using System;
using System.Collections;
using System.Collections.Generic;

using MEI.SPDocuments.Data;
using MEI.SPDocuments.TypeCodes;

namespace MEI.SPDocuments.Document
{
    [DocumentInfo(SPDocumentType.W9, "W9")]
    public class W9
        : SPDocumentBase, ISearchSpeaker, ISearchYear
    {
        internal W9(IRepository repository, IDbUtilities dbUtilities, DocumentTypeInfo documentTypeInfo)
            : base(repository, dbUtilities, documentTypeInfo)
        { }

        public W9 WithValues(int? speakerCounter, TinType tinType, DocumentYear documentYear)
        {
            SpeakerCounter = speakerCounter;
            TinType = tinType;
            DocumentYear = documentYear;

            return this;
        }

        [SPFieldInfo(SPFieldNames.DocumentYear, "Year", SPFieldType.Text, 1)]
        public override DocumentYear DocumentYear { get; internal set; }

        [SPFieldInfo(SPFieldNames.SpeakerCounter, "Program_x0020_ID", SPFieldType.Text, "SpeakerCounter", 0)]
        public int? SpeakerCounter { get; private set; }

        [SPFieldInfo(SPFieldNames.TinType, "TAXIDorSSN", SPFieldType.Choice, "TINSSN", 2)]
        public TinType TinType { get; private set; }

        public override string FileName => MakeFileName(SpeakerCounter, DocumentYear.ToDisplayNameLong(), TinType.ToDisplayNameShort());

        public override bool IsValid
        {
            get
            {
                bool baseValid = base.IsValid;

                if (!SpeakerCounter.HasValue)
                {
                    return false;
                }

                if (TinType == TinType.Undefined)
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

        public override string UniqueIdentifiers => "SpeakerCounter;DocumentYear;TinType";

        public override string UniqueValues =>
            string.Format("{0};{1};{2}", SpeakerCounter, DocumentYear.ToProgramIdYear(), TinType.ToDisplayNameShort());

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

            SpeakerCounter = Convert.ToInt32(objects[0]);
            DocumentYear = objects[1].ToString().ToDocumentYear();
            TinType = objects[2].ToString().ToTinType();
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

            if (values.ContainsKey(SPFields[SPFieldNames.TinType].InternalName))
            {
                TinType = values[SPFields[SPFieldNames.TinType].InternalName].ToString().ToTinType();
            }

            if (values.ContainsKey(SPFields[SPFieldNames.DocumentYear].InternalName))
            {
                DocumentYear = ((string)values[SPFields[SPFieldNames.DocumentYear].InternalName]).ToDocumentYear();
            }

            return true;
        }

        public override IDictionary<string, string> GetUserFieldValues()
        {
            return new Dictionary<string, string>
                   {
                       { SPFields[SPFieldNames.SpeakerCounter].InternalName, SpeakerCounter.ToString() },
                       { SPFields[SPFieldNames.DocumentYear].InternalName, DocumentYear.ToDisplayNameLong() },
                       { SPFields[SPFieldNames.TinType].InternalName, TinType.ToDisplayNameShort() }
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

            TinType = fileNameParts[3].ToTinType();

            if (TinType == TinType.Undefined)
            {
                ThrowFileNameExceptionInvalidType(fileNameToParse, SPFieldNames.TinType, "TinTypeCode");
            }

            DocumentYear = fileNameParts[2].ToDocumentYear();

            if (DocumentYear == DocumentYear.Undefined)
            {
                ThrowFileNameExceptionInvalidType(fileNameToParse, SPFieldNames.DocumentYear, "DocumentYear");
            }

            return fileNameParts;
        }
    }
}
