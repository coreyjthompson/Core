using System;
using System.Collections;
using System.Collections.Generic;

using MEI.SPDocuments.Data;
using MEI.SPDocuments.TypeCodes;

namespace MEI.SPDocuments.Document
{
    [DocumentEnabled(false)]
    [DocumentInfo(SPDocumentType.ThankYouLetter, "ThankYouLetter", "TYL", "ThankYouLetter", "Thank You Letter")]
    public class ThankYouLetter
        : SPDocumentBase, ISearchSpeaker, ISearchProgram, ISearchYear
    {
        internal ThankYouLetter(IRepository repository, IDbUtilities dbUtilities, DocumentTypeInfo documentTypeInfo)
            : base(repository, dbUtilities, documentTypeInfo)
        { }

        public ThankYouLetter WithValues(string programId, int? speakerCounter)
        {
            ProgramId = programId;
            SpeakerCounter = speakerCounter;

            return this;
        }

        public override DocumentYear DocumentYear => ExtractDocumentYear(ProgramId);

        [SPFieldInfo(SPFieldNames.ProgramId, "Program_x0020_ID", SPFieldType.Text, "Program ID", 0)]
        public string ProgramId { get; private set; }

        [SPFieldInfo(SPFieldNames.SpeakerCounter, "SpeakerCounter", SPFieldType.Text, 1)]
        public int? SpeakerCounter { get; private set; }

        public override string FileName => MakeFileName(ProgramId, SpeakerCounter);

        public override bool IsValid
        {
            get
            {
                bool baseValid = base.IsValid;

                if (string.IsNullOrEmpty(ProgramId))
                {
                    return false;
                }

                if (!SpeakerCounter.HasValue)
                {
                    return false;
                }

                return baseValid;
            }
        }

        public override string UniqueIdentifiers => "ProgramId;SpeakerCounter";

        public override string UniqueValues => string.Format("{0};{1}", ProgramId, SpeakerCounter);

        public ISearchExpressionGroup GetSearchExpressionGroupByProgram(Company company, DocumentYear year, string programId)
        {
            return new SearchExpressionGroup(this, SPFieldNames.ProgramId, CamlComparison.Equal, programId);
        }

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

            return new SearchExpressionGroup(this, SPFieldNames.ProgramId, CamlComparison.Contains, year.ToProgramIdYear());
        }

        public override bool ValidateFields()
        {
            if (!IsValid)
            {
                return false;
            }

            if (Repository.GetProgramIdsByProgramId(Company, DocumentYear, ProgramId).Rows.Count <= 0)
            {
                ThrowFileNameExceptionNoDBMatch(SPFieldNames.ProgramId, ProgramId);
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

            ProgramId = objects[0].ToString();
            SpeakerCounter = Convert.ToInt32(objects[1]);
            Contents = (byte[])objects[2];
            FileExtension = objects[3].ToString();
            Company = (Company)objects[4];

            return IsValid;
        }

        public override bool AbstractSetup(Hashtable values)
        {
            if (values.ContainsKey(SPFields[SPFieldNames.ProgramId].InternalName))
            {
                ProgramId = (string)values[SPFields[SPFieldNames.ProgramId].InternalName];
            }

            if (values.ContainsKey(SPFields[SPFieldNames.SpeakerCounter].InternalName))
            {
                SpeakerCounter = Convert.ToInt32(values[SPFields[SPFieldNames.SpeakerCounter].InternalName]);
            }

            return true;
        }

        public override IDictionary<string, string> GetUserFieldValues()
        {
            return new Dictionary<string, string>
                   {
                       { SPFields[SPFieldNames.ProgramId].InternalName, ProgramId },
                       { SPFields[SPFieldNames.SpeakerCounter].InternalName, SpeakerCounter.ToString() }
                   };
        }

        public override string[] ParseFileName(string fileNameToParse)
        {
            string[] fileNameParts = base.ParseFileName(fileNameToParse);

            ProgramId = fileNameParts[1];

            if (!int.TryParse(fileNameParts[2], out int tempSpeakerCounter))
            {
                ThrowFileNameExceptionInvalidType(fileNameToParse, SPFieldNames.SpeakerCounter, "Integer");
            }

            SpeakerCounter = tempSpeakerCounter;

            return fileNameParts;
        }
    }
}
