using System;
using System.Collections;
using System.Collections.Generic;

using MEI.SPDocuments.Data;
using MEI.SPDocuments.TypeCodes;

namespace MEI.SPDocuments.Document
{
    [DocumentInfo(SPDocumentType.HonoDisputeAmendment, "HonoDisputeAmendment", "HDA", "HDA", "Hono Dispute Amendment")]
    public class HonoDisputeAmendment
        : SPDocumentBase, ISearchExpense, ISearchSpeaker, ISearchProgram, ISearchYear
    {
        internal HonoDisputeAmendment(IRepository repository, IDbUtilities dbUtilities, DocumentTypeInfo documentTypeInfo)
            : base(repository, dbUtilities, documentTypeInfo)
        { }

        public HonoDisputeAmendment WithValues(string programId, int? speakerCounter, int? expenseCounter, int? disputeId)
        {
            ProgramId = programId;
            SpeakerCounter = speakerCounter;
            ExpenseCounter = expenseCounter;
            DisputeId = disputeId;

            return this;
        }

        public override DocumentYear DocumentYear => ExtractDocumentYear(ProgramId);

        [SPFieldInfo(SPFieldNames.DisputeId, "DisputeID", SPFieldType.Text, 3)]
        public int? DisputeId { get; private set; }

        [SPFieldInfo(SPFieldNames.ExpenseCounter, "ExpenseCounter", SPFieldType.Text, 2)]
        public int? ExpenseCounter { get; private set; }

        [SPFieldInfo(SPFieldNames.ProgramId, "ProgramID", SPFieldType.Text, 0)]
        public string ProgramId { get; private set; }

        [SPFieldInfo(SPFieldNames.SpeakerCounter, "SpkrCounter", SPFieldType.Text, 1)]
        public int? SpeakerCounter { get; private set; }

        public override string FileName => MakeFileName(ProgramId, SpeakerCounter);

        public override string ParsableFileName => MakeFileName(ProgramId, SpeakerCounter, ExpenseCounter, DisputeId);

        public override bool IsValid
        {
            get
            {
                bool baseValid = base.IsValid;

                if (string.IsNullOrEmpty(ProgramId))
                {
                    return false;
                }

                if (SpeakerCounter == null)
                {
                    return false;
                }

                if (!DisputeId.HasValue)
                {
                    return false;
                }

                return baseValid;
            }
        }

        public override string UniqueIdentifiers => "ProgramId;SpeakerCounter;ExpenseCounter;DisputeId";

        public override string UniqueValues => string.Format("{0};{1};{2};{3}", ProgramId, SpeakerCounter, ExpenseCounter, DisputeId);

        public ISearchExpressionGroup GetSearchExpressionGroupByExpense(Company company, DocumentYear year, int expenseCounter)
        {
            return new SearchExpressionGroup(this, SPFieldNames.ExpenseCounter, CamlComparison.Equal, expenseCounter);
        }

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
                ThrowFileNameExceptionNoDBMatch(SPFieldNames.SpeakerCounter, SpeakerCounter.ToString());
            }

            if (!DisputeId.HasValue)
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

            ProgramId = objects[0].ToString();
            SpeakerCounter = Convert.ToInt32(objects[1]);
            ExpenseCounter = Convert.ToInt32(objects[2]);
            DisputeId = Convert.ToInt32(objects[3]);
            Contents = (byte[])objects[4];
            FileExtension = objects[5].ToString();
            Company = (Company)objects[6];

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

            if (values.ContainsKey(SPFields[SPFieldNames.DisputeId].InternalName))
            {
                DisputeId = Convert.ToInt32(values[SPFields[SPFieldNames.DisputeId].InternalName]);
            }

            if (values.ContainsKey(SPFields[SPFieldNames.ExpenseCounter].InternalName))
            {
                ExpenseCounter = Convert.ToInt32(values[SPFields[SPFieldNames.ExpenseCounter].InternalName]);
            }

            return true;
        }

        public override IDictionary<string, string> GetUserFieldValues()
        {
            return new Dictionary<string, string>
                   {
                       { SPFields[SPFieldNames.ProgramId].InternalName, ProgramId },
                       { SPFields[SPFieldNames.SpeakerCounter].InternalName, SpeakerCounter.ToString() },
                       { SPFields[SPFieldNames.ExpenseCounter].InternalName, ExpenseCounter.ToString() },
                       { SPFields[SPFieldNames.DisputeId].InternalName, DisputeId.ToString() }
                   };
        }

        public override string[] ParseFileName(string fileNameToParse)
        {
            string[] fileNameParts = base.ParseFileName(fileNameToParse);

            //fileNameParts(4) = Regex.Match(fileNameParts(4), "([A-Z])", RegexOptions.IgnoreCase).Value

            ProgramId = fileNameParts[1];

            if (!int.TryParse(fileNameParts[2], out int tempSpeakerCounter))
            {
                ThrowFileNameExceptionInvalidType(fileNameToParse, SPFieldNames.SpeakerCounter, "Integer");
            }

            SpeakerCounter = tempSpeakerCounter;

            if (int.TryParse(fileNameParts[3], out int tempExpenseCounter))
            {
                ExpenseCounter = tempExpenseCounter;
            }

            if (!int.TryParse(fileNameParts[4], out int _))
            {
                ThrowFileNameExceptionInvalidType(fileNameToParse, SPFieldNames.DisputeId, "Integer");
            }

            DisputeId = Convert.ToInt32(fileNameParts[4]);

            return fileNameParts;
        }
    }
}
