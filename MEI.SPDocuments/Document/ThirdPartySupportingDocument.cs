using System.Collections;
using System.Collections.Generic;

using MEI.SPDocuments.Data;
using MEI.SPDocuments.TypeCodes;

namespace MEI.SPDocuments.Document
{
    [DocumentInfo(SPDocumentType.ThirdPartySupportingDocument,
        "ThirdPartySupportingDocument",
        "TPSD",
        "TPSD",
        "Third Party Supporting Document")]
    public class ThirdPartySupportingDocument
        : SPDocumentBase, ISearchProgram
    {
        internal ThirdPartySupportingDocument(IRepository repository, IDbUtilities dbUtilities, DocumentTypeInfo documentTypeInfo)
            : base(repository, dbUtilities, documentTypeInfo)
        { }

        public ThirdPartySupportingDocument WithValues(string programId, string fileType, string description)
        {
            ProgramId = programId;
            FileType = fileType;
            Description = description;

            return this;
        }

        public override DocumentYear DocumentYear => ExtractDocumentYear(ProgramId);

        public override string SearchFieldValue => ProgramId;

        [SPFieldInfo(SPFieldNames.ProgramId, "ProgramId", SPFieldType.Text, "ProgramId", 0)]
        public string ProgramId { get; private set; }

        [SPFieldInfo(SPFieldNames.FileType, "FileType", SPFieldType.Text, "FileType", 1)]
        public string FileType { get; private set; }

        [SPFieldInfo(SPFieldNames.Description, "Description0", SPFieldType.Text, "Description", 2)]
        public string Description { get; private set; }

        public override string FileName => MakeFileName(ProgramId, Description, FileType);

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

        public override string UniqueIdentifiers => "Description,ProgramId;FileType;";

        public override string UniqueValues => string.Format("{0};{1};{2}", Description, ProgramId, FileType);

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
            FileType = objects[1].ToString();
            Description = objects[3].ToString();
            Contents = (byte[])objects[4];
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

            if (values.ContainsKey(SPFields[SPFieldNames.FileType].InternalName))
            {
                FileType = (string)values[SPFields[SPFieldNames.FileType].InternalName];
            }

            if (values.ContainsKey(SPFields[SPFieldNames.Description].InternalName))
            {
                Description = (string)values[SPFields[SPFieldNames.Description].InternalName];
            }

            return IsValid;
        }

        public override IDictionary<string, string> GetUserFieldValues()
        {
            return new Dictionary<string, string>
                   {
                       { SPFields[SPFieldNames.ProgramId].InternalName, ProgramId },
                       { SPFields[SPFieldNames.FileType].InternalName, FileType },
                       { SPFields[SPFieldNames.Description].InternalName, Description }
                   };
        }

        public override string[] ParseFileName(string fileNameToParse)
        {
            string[] fileNameParts = base.ParseFileName(fileNameToParse);

            ProgramId = fileNameParts[1];
            Description = fileNameParts[2];
            FileType = fileNameParts[3];

            return fileNameParts;
        }
    }
}
