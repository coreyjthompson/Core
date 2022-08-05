using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

using MEI.SPDocuments.Data;
using MEI.SPDocuments.TypeCodes;

namespace MEI.SPDocuments.Document
{
    [DocumentInfo(SPDocumentType.RepReceipt, "RepReceipt", "RRCT")]
    public class RepReceipt
        : SPDocumentBase, ISearchExpense, ISearchProgram, ISearchVendor, ISearchYear
    {
        internal RepReceipt(IRepository repository, IDbUtilities dbUtilities, DocumentTypeInfo documentTypeInfo)
            : base(repository, dbUtilities, documentTypeInfo)
        { }

        public RepReceipt WithValues(string programId, int? expenseCounter)
        {
            ExpenseCounter = expenseCounter;
            ProgramId = programId;

            return this;
        }

        public override DocumentYear DocumentYear => ExtractDocumentYear(ProgramId);

        public override string SearchFieldValue => ProgramId;

        [SPFieldInfo(SPFieldNames.ExpenseCounter, "ExpenseCounter", SPFieldType.Text, 1)]
        public int? ExpenseCounter { get; private set; }

        [SPFieldInfo(SPFieldNames.ProgramId, "ProgramID", SPFieldType.Text, 0)]
        public string ProgramId { get; private set; }

        public override string FileName => MakeFileName(ProgramId, ExpenseCounter);

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

                return baseValid;
            }
        }

        public override string UniqueIdentifiers => "ProgramId;ExpenseCounter";

        public override string UniqueValues => string.Format("{0};{1}", ProgramId, ExpenseCounter);

        public ISearchExpressionGroup GetSearchExpressionGroupByExpense(Company company, DocumentYear year, int expenseCounter)
        {
            return new SearchExpressionGroup(this, SPFieldNames.ExpenseCounter, CamlComparison.Equal, expenseCounter);
        }

        public ISearchExpressionGroup GetSearchExpressionGroupByProgram(Company company, DocumentYear year, string programId)
        {
            return new SearchExpressionGroup(this, SPFieldNames.ProgramId, CamlComparison.Equal, programId);
        }

        public ISearchExpressionGroup GetSearchExpressionGroupByVendor(Company company, DocumentYear year, int vendorId)
        {
            DataTable dt = Repository.GetExpenseCountersByVendorId(company, year, vendorId);

            var seg = new SearchExpressionGroup(this)
                      {
                          BooleanLogicType = SearchBooleanLogic.Or
                      };

            foreach (DataRow dr in dt.Rows)
            {
                seg.AddExpression(SPFieldNames.ExpenseCounter, CamlComparison.Equal, DbUtilities.FromDbValue<int>(dr["ExpenseCounter"]));
            }

            return seg;
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
            Contents = (byte[])objects[2];
            FileExtension = objects[3].ToString();
            Company = (Company)objects[4];

            return IsValid;
        }

        public override bool AbstractSetup(Hashtable values)
        {
            if (values.ContainsKey(SPFields[SPFieldNames.ExpenseCounter].InternalName))
            {
                ExpenseCounter = Convert.ToInt32(values[SPFields[SPFieldNames.ExpenseCounter].InternalName]);
            }

            if (values.ContainsKey(SPFields[SPFieldNames.ProgramId].InternalName))
            {
                ProgramId = (string)values[SPFields[SPFieldNames.ProgramId].InternalName];
            }

            return true;
        }

        public override IDictionary<string, string> GetUserFieldValues()
        {
            return new Dictionary<string, string>
                   {
                       { SPFields[SPFieldNames.ProgramId].InternalName, ProgramId },
                       { SPFields[SPFieldNames.ExpenseCounter].InternalName, ExpenseCounter.ToString() }
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

            return fileNameParts;
        }
    }
}
