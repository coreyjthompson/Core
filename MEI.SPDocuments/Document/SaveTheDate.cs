using System;
using System.Collections;
using System.Collections.Generic;

using MEI.SPDocuments.Data;
using MEI.SPDocuments.TypeCodes;

namespace MEI.SPDocuments.Document
{
    [DocumentInfo(SPDocumentType.SaveTheDate, "SaveTheDate", "STD", "STD", "Save The Date")]
    public class SaveTheDate
        : SPDocumentBase, ISearchProgram, ISearchYear
    {
        internal SaveTheDate(IRepository repository, IDbUtilities dbUtilities, DocumentTypeInfo documentTypeInfo)
            : base(repository, dbUtilities, documentTypeInfo)
        { }

        public SaveTheDate WithValues(string programId, int? saveTheDateId)
        {
            ProgramId = programId;
            SaveTheDateId = saveTheDateId;

            return this;
        }

        public override DocumentYear DocumentYear => ExtractDocumentYear(ProgramId);

        [SPFieldInfo(SPFieldNames.ProgramId, "Program_x0020_ID", SPFieldType.Text, "ProgramID", 1)]
        public string ProgramId { get; private set; }

        [SPFieldInfo(SPFieldNames.SaveTheDateId, "STDID", SPFieldType.Text, 0)]
        public int? SaveTheDateId { get; private set; }

        public override string FileName => MakeFileName(ProgramId, SaveTheDateId);

        public override bool IsValid
        {
            get
            {
                bool baseValid = base.IsValid;

                if (string.IsNullOrEmpty(ProgramId))
                {
                    return false;
                }

                if (!SaveTheDateId.HasValue)
                {
                    return false;
                }

                return baseValid;
            }
        }

        public override string UniqueIdentifiers => "SaveTheDateId;ProgramId";

        public override string UniqueValues => string.Format("{0};{1}", SaveTheDateId, ProgramId);

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

            if (SaveTheDateId != null && Repository.GetSaveTheDateIdsBySaveTheDateId(Company, DocumentYear, SaveTheDateId.Value).Rows.Count <= 0)
            {
                ThrowFileNameExceptionNoDBMatch(SPFieldNames.SaveTheDateId, SaveTheDateId.Value.ToString());
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
            SaveTheDateId = Convert.ToInt32(objects[1]);
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

            if (values.ContainsKey(SPFields[SPFieldNames.SaveTheDateId].InternalName))
            {
                SaveTheDateId = Convert.ToInt32(values[SPFields[SPFieldNames.SaveTheDateId].InternalName]);
            }

            return true;
        }

        public override IDictionary<string, string> GetUserFieldValues()
        {
            return new Dictionary<string, string>
                   {
                       { SPFields[SPFieldNames.ProgramId].InternalName, ProgramId },
                       { SPFields[SPFieldNames.SaveTheDateId].InternalName, SaveTheDateId.ToString() }
                   };
        }

        public override string[] ParseFileName(string fileNameToParse)
        {
            string[] fileNameParts = base.ParseFileName(fileNameToParse);

            ProgramId = fileNameParts[1];

            if (!int.TryParse(fileNameParts[2], out int tempStdId))
            {
                ThrowFileNameExceptionInvalidType(fileNameToParse, SPFieldNames.SaveTheDateId, "Integer");
            }

            SaveTheDateId = tempStdId;

            return fileNameParts;
        }
    }
}
