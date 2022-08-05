using System;
using System.Collections;
using System.Collections.Generic;

using MEI.SPDocuments.Data;
using MEI.SPDocuments.TypeCodes;

namespace MEI.SPDocuments.Document
{
    [DocumentInfo(SPDocumentType.PharmaceuticalDetailersLicense,
        "PharmaceuticalDetailersLicense",
        "PDL",
        "PDL",
        "Pharmaceutical Detailers License")]
    public class PharmaceuticalDetailersLicense
        : SPDocumentBase, ISearchSpeaker, ISearchYear
    {
        internal PharmaceuticalDetailersLicense(IRepository repository, IDbUtilities dbUtilities, DocumentTypeInfo documentTypeInfo)
            : base(repository, dbUtilities, documentTypeInfo)
        { }

        public PharmaceuticalDetailersLicense WithValues(int? speakerCounter, USState stateType, DocumentYear documentYear)
        {
            SpeakerCounter = speakerCounter;
            State = stateType;
            DocumentYear = documentYear;

            return this;
        }

        [SPFieldInfo(SPFieldNames.DocumentYear, "DocumentYear", SPFieldType.Text, 2)]
        public override DocumentYear DocumentYear {get; internal set; }

        public override string ParsableFileName =>
            MakeFileName(SpeakerCounter, State.ToDisplayNameShort(), DocumentYear.ToDisplayNameLong());

        [SPFieldInfo(SPFieldNames.SpeakerCounter, "Speaker_x0020_Counter", SPFieldType.Text, "Speaker Counter", 0)]
        public int? SpeakerCounter { get; private set; }

        [SPFieldInfo(SPFieldNames.State, "State", SPFieldType.Text, 1)]
        public USState State { get; private set; }

        public override string FileName => MakeFileName(SpeakerCounter, State.ToDisplayNameShort());

        public override bool IsValid
        {
            get
            {
                bool baseValid = base.IsValid;

                if (!SpeakerCounter.HasValue)
                {
                    return false;
                }

                if (State == USState.Undefined)
                {
                    return false;
                }

                return baseValid;
            }
        }

        public override string UniqueIdentifiers => "SpeakerCounter;State;DocumentYear";

        public override string UniqueValues =>
            string.Format("{0};{1};{2}", SpeakerCounter, State.ToDisplayNameShort(), DocumentYear.ToDisplayNameLong());

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

            return new SearchExpressionGroup(this, SPFieldNames.DocumentYear, CamlComparison.Contains, year.ToDisplayNameLong());
        }

        public override bool ValidateFields()
        {
            if (!IsValid)
            {
                return false;
            }

            if (SpeakerCounter != null && Repository.GetSpeakerCountersBySpeakerCounter(Company, DocumentYear, SpeakerCounter.Value).Rows.Count <= 0)
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

            SpeakerCounter = Convert.ToInt32(objects[0]);
            State = objects[1].ToString().ToUSState();
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

            if (values.ContainsKey(SPFields[SPFieldNames.State].InternalName))
            {
                State = ((string)values[SPFields[SPFieldNames.State].InternalName]).ToUSState();
            }

            if (values.ContainsKey(SPFields[SPFieldNames.DocumentYear].InternalName))
            {
                DocumentYear = Convert.ToString(values[SPFields[SPFieldNames.DocumentYear].InternalName]).ToDocumentYear();
            }

            return true;
        }

        public override IDictionary<string, string> GetUserFieldValues()
        {
            return new Dictionary<string, string>
                   {
                       { SPFields[SPFieldNames.SpeakerCounter].InternalName, SpeakerCounter.ToString() },
                       { SPFields[SPFieldNames.State].InternalName, State.ToDisplayNameShort() },
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

            State = fileNameParts[2].ToUSState();

            string tempDocumentYear = fileNameParts[3];
            DocumentYear = tempDocumentYear.ToDocumentYear();

            return fileNameParts;
        }
    }
}
