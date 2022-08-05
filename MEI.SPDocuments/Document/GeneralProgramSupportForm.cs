using System.Collections;
using System.Collections.Generic;

using MEI.SPDocuments.Data;
using MEI.SPDocuments.TypeCodes;

namespace MEI.SPDocuments.Document
{
    [DocumentInfo(SPDocumentType.GeneralProgramSupportForm,
        "GeneralProgramSupportForm",
        "TPGPSF",
        "TPGPSF",
        "General Program Support Form")]
    public class GeneralProgramSupportForm
        : SPDocumentBase, ISearchProgram
    {
        internal GeneralProgramSupportForm(IRepository repository, IDbUtilities dbUtilities, DocumentTypeInfo documentTypeInfo)
            : base(repository, dbUtilities, documentTypeInfo)
        { }

        public GeneralProgramSupportForm WithValues(string programId, string documentType, string description)
        {
            ProgramId = programId;
            DocumentType = documentType;
            Description = description;

            return this;
        }

        public override DocumentYear DocumentYear => ExtractDocumentYear(ProgramId);

        public override string SearchFieldValue => ProgramId;

        [SPFieldInfo(SPFieldNames.ProgramId, "ProgramID", SPFieldType.Text, "ProgramID", 0)]
        public string ProgramId { get; private set; }

        [SPFieldInfo(SPFieldNames.DocumentType, "DocumentType", SPFieldType.Text, "DocumentType", 1)]
        public new string DocumentType { get; private set; }

        [SPFieldInfo(SPFieldNames.Description, "Description0", SPFieldType.Text, "Description", 2)]
        public string Description { get; private set; }

        public override string FileName => MakeFileName(ProgramId, Description, DocumentType);

        public override bool IsValid
        {
            get
            {
                bool baseValid = base.IsValid;

                if (string.IsNullOrEmpty(ProgramId))
                {
                    return false;
                }

                return baseValid;
            }
        }

        public override string UniqueIdentifiers => "ProgramId;DocumentType;Description";

        public override string UniqueValues => string.Format("{0};{1};{2}", ProgramId, DocumentType, Description);

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

            if (Repository.GetProgramIdsByProgramId(Company, DocumentYear, ProgramId).Rows.Count <= 0)
            {
                ThrowFileNameExceptionNoDBMatch(SPFieldNames.ProgramId, ProgramId);
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
            Description = objects[1].ToString();
            DocumentType = objects[2].ToString();
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

            if (values.ContainsKey(SPFields[SPFieldNames.DocumentType].InternalName))
            {
                DocumentType = (string)values[SPFields[SPFieldNames.DocumentType].InternalName];
            }

            if (values.ContainsKey(SPFields[SPFieldNames.Description].InternalName))
            {
                Description = (string)values[SPFields[SPFieldNames.Description].InternalName];
            }

            return true;
        }

        public override IDictionary<string, string> GetUserFieldValues()
        {
            return new Dictionary<string, string>
                   {
                       { SPFields[SPFieldNames.ProgramId].InternalName, ProgramId },
                       { SPFields[SPFieldNames.DocumentType].InternalName, DocumentType },
                       { SPFields[SPFieldNames.Description].InternalName, Description }
                   };
        }

        public override string[] ParseFileName(string fileNameToParse)
        {
            string[] fileNameParts = base.ParseFileName(fileNameToParse);

            ProgramId = fileNameParts[1];
            Description = fileNameParts[2];
            DocumentType = fileNameParts[3];

            return fileNameParts;
        }
    }
}
