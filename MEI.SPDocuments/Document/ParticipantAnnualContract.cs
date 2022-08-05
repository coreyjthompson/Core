using System;
using System.Collections;
using System.Collections.Generic;

using MEI.SPDocuments.Data;
using MEI.SPDocuments.TypeCodes;

namespace MEI.SPDocuments.Document
{
    [DocumentInfo(SPDocumentType.ParticipantAnnualContract, "ParticipantAnnualContract", "PAC", "PAC", "Participant Annual Contract")]
    public class ParticipantAnnualContract
        : SPDocumentBase, ISearchProgram
    {
        internal ParticipantAnnualContract(IRepository repository, IDbUtilities dbUtilities, DocumentTypeInfo documentTypeInfo)
            : base(repository, dbUtilities, documentTypeInfo)
        { }

        public ParticipantAnnualContract WithValues(int? participantCounter, string programId, DocumentYear documentYear)
        {
            ParticipantCounter = participantCounter;
            ProgramId = programId;
            DocumentYear = documentYear;

            return this;
        }

        [SPFieldInfo(SPFieldNames.DocumentYear, "Year", SPFieldType.Text, 2)]
        public override DocumentYear DocumentYear { get; internal set; }

        [SPFieldInfo(SPFieldNames.ParticipantCounter, "ParticipantCounter", SPFieldType.Text, 0)]
        public int? ParticipantCounter { get; private set; }

        [SPFieldInfo(SPFieldNames.ProgramId, "ProgramID", SPFieldType.Text, 1)]
        public string ProgramId { get; private set; }

        public override string FileName => MakeFileName(ParticipantCounter, ProgramId, DocumentYear.ToDisplayNameLong());

        public override bool IsValid
        {
            get
            {
                bool baseValid = base.IsValid;

                if (!ParticipantCounter.HasValue)
                {
                    return false;
                }

                if (string.IsNullOrEmpty(ProgramId))
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

        public override string UniqueIdentifiers => "ParticipantCounter;ProgramId;DocumentYear";

        public override string UniqueValues => string.Format("{0};{1};{2}", ParticipantCounter, ProgramId, DocumentYear.ToProgramIdYear());

        public ISearchExpressionGroup GetSearchExpressionGroupByProgram(Company company, DocumentYear year, string programId)
        {
            return new SearchExpressionGroup(this, SPFieldNames.ProgramId, CamlComparison.Equal, programId);
        }

        public override bool ValidateFields()
        {
            if (!IsValid)
            {
                return false;
            }

            if (ParticipantCounter != null && Repository.GetParticipantCounters(Company, DocumentYear, ParticipantCounter.Value).Rows.Count <= 0)
            {
                ThrowFileNameExceptionNoDBMatch(SPFieldNames.ParticipantCounter, ParticipantCounter.Value.ToString());
            }
            else if (Repository.GetProgramIdsByProgramId(Company, DocumentYear, ProgramId).Rows.Count <= 0)
            {
                ThrowFileNameExceptionNoDBMatch(SPFieldNames.ProgramId, ProgramId);
            }
            else if (DocumentYear == DocumentYear.Undefined)
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

            ParticipantCounter = Convert.ToInt32(objects[0]);
            ProgramId = Convert.ToString(objects[1]);

            DocumentYear = objects[2].ToString().ToDocumentYear();
            Contents = (byte[])objects[3];
            FileExtension = objects[4].ToString();
            Company = (Company)objects[5];

            return IsValid;
        }

        public override bool AbstractSetup(Hashtable values)
        {
            if (values.ContainsKey(SPFields[SPFieldNames.ParticipantCounter].InternalName))
            {
                ParticipantCounter = Convert.ToInt32(values[SPFields[SPFieldNames.ParticipantCounter].InternalName]);
            }

            if (values.ContainsKey(SPFields[SPFieldNames.ProgramId].InternalName))
            {
                ProgramId = Convert.ToString(values[SPFields[SPFieldNames.ProgramId].InternalName]);
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
                       { SPFields[SPFieldNames.ParticipantCounter].InternalName, ParticipantCounter.ToString() },
                       { SPFields[SPFieldNames.ProgramId].InternalName, ProgramId },
                       { SPFields[SPFieldNames.DocumentYear].InternalName, DocumentYear.ToDisplayNameLong() }
                   };
        }

        public override string[] ParseFileName(string fileNameToParse)
        {
            string[] fileNameParts = base.ParseFileName(fileNameToParse);

            if (!int.TryParse(fileNameParts[1], out int tempParticipantCounter))
            {
                ThrowFileNameExceptionInvalidType(fileNameToParse, SPFieldNames.ParticipantCounter, "Integer");
            }

            ParticipantCounter = tempParticipantCounter;

            ProgramId = fileNameParts[2];

            DocumentYear = fileNameParts[3].ToDocumentYear();
            if (DocumentYear == DocumentYear.Undefined)
            {
                ThrowFileNameExceptionInvalidType(fileNameToParse, SPFieldNames.DocumentYear, "DocumentYear");
            }

            return fileNameParts;
        }
    }
}
