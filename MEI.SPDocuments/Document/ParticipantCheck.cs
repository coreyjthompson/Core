using System;
using System.Collections;
using System.Collections.Generic;

using MEI.SPDocuments.Data;
using MEI.SPDocuments.TypeCodes;

namespace MEI.SPDocuments.Document
{
    [DocumentInfo(SPDocumentType.ParticipantCheck, "ParticipantCheck", "PC", "PC", "Participant Check")]
    public class ParticipantCheck
        : SPDocumentBase, ISearchProgram, ISearchExpense
    {
        internal ParticipantCheck(IRepository repository, IDbUtilities dbUtilities, DocumentTypeInfo documentTypeInfo)
            : base(repository, dbUtilities, documentTypeInfo)
        { }

        public ParticipantCheck WithValues(int? participantCounter, string programId, int? expenseCounter, CheckType checkType)
        {
            ParticipantCounter = participantCounter;
            ProgramId = programId;
            ExpenseCounter = expenseCounter;
            CheckType = checkType;

            return this;
        }

        public override DocumentYear DocumentYear => ExtractDocumentYear(ProgramId);

        public override string SearchFieldValue => ProgramId;

        [SPFieldInfo(SPFieldNames.ParticipantCounter, "ParticipantCounter", SPFieldType.Text, 0)]
        public int? ParticipantCounter { get; private set; }

        [SPFieldInfo(SPFieldNames.ProgramId, "Program_x0020_ID", SPFieldType.Text, 1)]
        public string ProgramId { get; private set; }

        [SPFieldInfo(SPFieldNames.ExpenseCounter, "ExpCounter", SPFieldType.Text, 2)]
        public int? ExpenseCounter { get; private set; }

        [SPFieldInfo(SPFieldNames.CheckType, "CheckType", SPFieldType.Text, 3)]
        public CheckType CheckType { get; private set; }

        public override string FileName => MakeFileName(ProgramId, ParticipantCounter, ExpenseCounter, CheckType.ToDisplayNameShort());

        public override bool IsValid
        {
            get
            {
                bool baseValid = base.IsValid;

                if (string.IsNullOrEmpty(ProgramId))
                {
                    return false;
                }

                if (!ParticipantCounter.HasValue)
                {
                    return false;
                }

                if (!ExpenseCounter.HasValue)
                {
                    return false;
                }

                if (CheckType == CheckType.Undefined)
                {
                    return false;
                }

                return baseValid;
            }
        }

        public override string UniqueIdentifiers => "ParticipantCounter;ProgramId;ExpenseCounter;CheckType";

        public override string UniqueValues =>
            string.Format("{0};{1};{2};{3}", ParticipantCounter, ProgramId, ExpenseCounter, CheckType.ToDisplayNameLong());

        public ISearchExpressionGroup GetSearchExpressionGroupByExpense(Company company, DocumentYear year, int expenseCounter)
        {
            return new SearchExpressionGroup(this, SPFieldNames.ExpenseCounter, CamlComparison.Equal, expenseCounter);
        }

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

            if (Repository.GetProgramIdsByProgramId(Company, DocumentYear, ProgramId).Rows.Count <= 0)
            {
                ThrowFileNameExceptionNoDBMatch(SPFieldNames.ProgramId, ProgramId);
            }

            if (ExpenseCounter != null && Repository.GetExpenseCountersByExpenseCounter(Company, DocumentYear, ExpenseCounter.Value).Rows.Count <= 0)
            {
                ThrowFileNameExceptionNoDBMatch(SPFieldNames.ExpenseCounter, ExpenseCounter.Value.ToString());
            }

            //If Not Validator.ValidateCheckTypeAcronym(Company, DocumentYear, CheckType) Then
            //	'TODO: make check type validator
            //	Return False
            //End If

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
            ParticipantCounter = Convert.ToInt32(objects[1]);
            ExpenseCounter = Convert.ToInt32(objects[2]);
            CheckType = (CheckType)objects[3];
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

            if (values.ContainsKey(SPFields[SPFieldNames.ParticipantCounter].InternalName))
            {
                ParticipantCounter = Convert.ToInt32(values[SPFields[SPFieldNames.ParticipantCounter].InternalName]);
            }

            if (values.ContainsKey(SPFields[SPFieldNames.ExpenseCounter].InternalName))
            {
                ExpenseCounter = Convert.ToInt32(values[SPFields[SPFieldNames.ExpenseCounter].InternalName]);
            }

            if (values.ContainsKey(SPFields[SPFieldNames.CheckType].InternalName))
            {
                CheckType = ((string)values[SPFields[SPFieldNames.CheckType].InternalName]).ToCheckType();
            }

            return true;
        }

        public override IDictionary<string, string> GetUserFieldValues()
        {
            return new Dictionary<string, string>
                   {
                       { SPFields[SPFieldNames.ProgramId].InternalName, ProgramId },
                       { SPFields[SPFieldNames.ParticipantCounter].InternalName, ParticipantCounter.ToString() },
                       { SPFields[SPFieldNames.ExpenseCounter].InternalName, ExpenseCounter.ToString() },
                       { SPFields[SPFieldNames.CheckType].InternalName, CheckType.ToDisplayNameShort() }
                   };
        }

        public override string[] ParseFileName(string fileNameToParse)
        {
            string[] fileNameParts = base.ParseFileName(fileNameToParse);

            ProgramId = fileNameParts[1];

            if (!int.TryParse(fileNameParts[2], out int tempParticipantCounter))
            {
                ThrowFileNameExceptionInvalidType(fileNameToParse, SPFieldNames.ParticipantCounter, "Integer");
            }

            ParticipantCounter = tempParticipantCounter;

            if (!int.TryParse(fileNameParts[3], out int tempExpCounter))
            {
                ThrowFileNameExceptionInvalidType(fileNameToParse, SPFieldNames.ExpenseCounter, "Integer");
            }

            ExpenseCounter = tempExpCounter;

            CheckType = fileNameParts[4].ToCheckType();

            if (CheckType == CheckType.Undefined)
            {
                ThrowFileNameExceptionInvalidType(fileNameToParse, SPFieldNames.CheckType, "CheckType");
            }

            return fileNameParts;
        }
    }
}
