using System;
using System.Collections;
using System.Collections.Generic;

using MEI.SPDocuments.Data;
using MEI.SPDocuments.TypeCodes;

namespace MEI.SPDocuments.Document
{
    [DocumentInfo(SPDocumentType.ProgramCheck, "ProgramCheck", "PRCK", "ProgramCheck", "Program Check")]
    public class ProgramCheck
        : SPDocumentBase, ISearchExpense, ISearchProgram, ISearchYear
    {
        internal ProgramCheck(IRepository repository, IDbUtilities dbUtilities, DocumentTypeInfo documentTypeInfo)
            : base(repository, dbUtilities, documentTypeInfo)
        { }

        public ProgramCheck WithValues(string programId, int? expenseCounter, CheckType checkType)
        {
            CheckType = checkType;
            ExpenseCounter = expenseCounter;
            ProgramId = programId;

            return this;
        }

        public override DocumentYear DocumentYear => ExtractDocumentYear(ProgramId);

        [SPFieldInfo(SPFieldNames.CheckType, "CheckType", SPFieldType.Choice, 2)]
        public CheckType CheckType { get; private set; }

        [SPFieldInfo(SPFieldNames.ExpenseCounter, "ExpCounter", SPFieldType.Text, 1)]
        public int? ExpenseCounter { get; private set; }

        [SPFieldInfo(SPFieldNames.ProgramId, "ProgramID", SPFieldType.Text, "Program ID", 0)]
        public string ProgramId { get; private set; }

        public override string FileName => MakeFileName(ProgramId, ExpenseCounter, CheckType.ToDisplayNameShort());

        public override bool IsValid
        {
            get
            {
                bool baseValid = base.IsValid;

                if (string.IsNullOrEmpty(ProgramId))
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

        public override string UniqueIdentifiers => "ProgramId;ExpenseCounter;CheckType";

        public override string UniqueValues => string.Format("{0};{1};{2}", ProgramId, ExpenseCounter, CheckType.ToDisplayNameLong());

        public ISearchExpressionGroup GetSearchExpressionGroupByExpense(Company company, DocumentYear year, int expenseCounter)
        {
            return new SearchExpressionGroup(this, SPFieldNames.ExpenseCounter, CamlComparison.Equal, expenseCounter);
        }

        public ISearchExpressionGroup GetSearchExpressionGroupByProgram(Company company, DocumentYear year, string programId)
        {
            return new SearchExpressionGroup(this, SPFieldNames.ProgramId, CamlComparison.Equal, programId);
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

            if (ExpenseCounter != null && Repository.GetExpenseCountersByExpenseCounter(Company, DocumentYear, ExpenseCounter.Value).Rows.Count <= 0)
            {
                ThrowFileNameExceptionNoDBMatch(SPFieldNames.ExpenseCounter, ExpenseCounter.Value.ToString());
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
            ExpenseCounter = Convert.ToInt32(objects[1]);
            CheckType = objects[2].ToString().ToCheckType();
            Contents = (byte[])objects[3];
            FileExtension = objects[4].ToString();
            Company = (Company)objects[5];

            return IsValid;
        }

        public override bool AbstractSetup(Hashtable values)
        {
            if (values.ContainsKey(SPFields[SPFieldNames.ProgramId].InternalName))
            {
                ProgramId = (string)values[SPFields[SPFieldNames.ProgramId].InternalName];
            }

            if (values.ContainsKey(SPFields[SPFieldNames.CheckType].InternalName))
            {
                CheckType = ((string)values[SPFields[SPFieldNames.CheckType].InternalName]).ToCheckType();
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
                       { SPFields[SPFieldNames.ExpenseCounter].InternalName, ExpenseCounter.ToString() },
                       { SPFields[SPFieldNames.CheckType].InternalName, CheckType.ToDisplayNameShort() }
                   };
        }

        public override string[] ParseFileName(string fileNameToParse)
        {
            string[] fileNameParts = base.ParseFileName(fileNameToParse);

            ProgramId = fileNameParts[1];

            if (!int.TryParse(fileNameParts[2], out int tempExpenseCounter))
            {
                ThrowFileNameExceptionInvalidType(fileNameToParse, SPFieldNames.ExpenseCounter, "Integer");
            }

            ExpenseCounter = tempExpenseCounter;

            CheckType = fileNameParts[3].ToCheckType();

            if (CheckType == CheckType.Undefined)
            {
                ThrowFileNameExceptionInvalidType(fileNameToParse, SPFieldNames.CheckType, "CheckTypeCode");
            }

            return fileNameParts;
        }
    }
}
