using System.Collections;
using System.Collections.Generic;

using MEI.SPDocuments.Data;
using MEI.SPDocuments.TypeCodes;

namespace MEI.SPDocuments.Document
{
    [DocumentInfo(SPDocumentType.ThirdPartyEventApproval, "ThirdPartyEventApproval", "TPEA")]
    public class ThirdPartyEventApproval
        : SPDocumentBase, ISearchProgram, ISearchYear
    {
        internal ThirdPartyEventApproval(IRepository repository, IDbUtilities dbUtilities, DocumentTypeInfo documentTypeInfo)
            : base(repository, dbUtilities, documentTypeInfo)
        { }

        public ThirdPartyEventApproval WithValues(string approvalId, string programId)
        {
            ApprovalId = approvalId;
            ProgramId = programId;

            return this;
        }

        public override DocumentYear DocumentYear => ExtractDocumentYear(ProgramId);

        public override string SearchFieldValue => ProgramId;

        [SPFieldInfo(SPFieldNames.ApprovalId, "ApprovalID", SPFieldType.Text, "ApprovalID", 0)]
        public string ApprovalId { get; private set; }

        [SPFieldInfo(SPFieldNames.ProgramId, "ProgramID", SPFieldType.Text, "ProgramID", 1)]
        public string ProgramId { get; private set; }

        public override string FileName => MakeFileName(ApprovalId, ProgramId);

        public override bool IsValid
        {
            get
            {
                bool baseValid = base.IsValid;

                if (string.IsNullOrEmpty(ProgramId))
                {
                    return false;
                }

                if (string.IsNullOrEmpty(ApprovalId))
                {
                    return false;
                }

                return baseValid;
            }
        }

        public override string UniqueIdentifiers => "ApprovalId;ProgramId";

        public override string UniqueValues => string.Format("{0};{1}", ApprovalId, ProgramId);

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

            if (Repository.GetApprovalIdByProgramId(Company, DocumentYear, ProgramId, ApprovalId).Rows.Count <= 0)
            {
                ThrowFileNameExceptionNoDBMatch(SPFieldNames.ApprovalId, ApprovalId);
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

            ApprovalId = objects[0].ToString();
            ProgramId = objects[1].ToString();
            Contents = (byte[])objects[2];
            FileExtension = objects[3].ToString();
            Company = (Company)objects[4];

            return IsValid;
        }

        public override bool AbstractSetup(Hashtable values)
        {
            if (values.ContainsKey(SPFields[SPFieldNames.ApprovalId].InternalName))
            {
                ApprovalId = (string)values[SPFields[SPFieldNames.ApprovalId].InternalName];
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
                       { SPFields[SPFieldNames.ApprovalId].InternalName, ApprovalId },
                       { SPFields[SPFieldNames.ProgramId].InternalName, ProgramId }
                   };
        }

        public override string[] ParseFileName(string fileNameToParse)
        {
            string[] fileNameParts = base.ParseFileName(fileNameToParse);

            ApprovalId = fileNameParts[1];
            ProgramId = fileNameParts[2];

            return fileNameParts;
        }
    }
}
