using System;
using System.Collections;
using System.Collections.Generic;

using MEI.SPDocuments.Data;
using MEI.SPDocuments.TypeCodes;

namespace MEI.SPDocuments.Document
{
    [DocumentInfo(SPDocumentType.Handout, "Handout", "HO")]
    public class Handout
        : SPDocumentBase, ISearchProgram, ISearchYear
    {
        internal Handout(IRepository repository, IDbUtilities dbUtilities, DocumentTypeInfo documentTypeInfo)
            : base(repository, dbUtilities, documentTypeInfo)
        { }

        public Handout WithValues(string programId, int? handoutId)
        {
            ProgramId = programId;
            HandoutId = handoutId;

            return this;
        }

        public override DocumentYear DocumentYear => ExtractDocumentYear(ProgramId);

        [SPFieldInfo(SPFieldNames.ProgramId, "ProgramId", SPFieldType.Text, 0)]
        public string ProgramId { get; private set; }

        [SPFieldInfo(SPFieldNames.HandoutId, "HandoutId", SPFieldType.Text, 1)]
        public int? HandoutId { get; private set; }

        public override string FileName => MakeFileName(ProgramId, HandoutId);

        public override bool IsValid
        {
            get
            {
                bool baseValid = base.IsValid;

                if (string.IsNullOrEmpty(ProgramId))
                {
                    return false;
                }

                if (!HandoutId.HasValue)
                {
                    return false;
                }

                return baseValid;
            }
        }

        public override string UniqueIdentifiers => "ProgramId;HandoutId";

        public override string UniqueValues => string.Format("{0};{1}", ProgramId, HandoutId);

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
            HandoutId = Convert.ToInt32(objects[1]);
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

            if (values.ContainsKey(SPFields[SPFieldNames.HandoutId].InternalName))
            {
                HandoutId = Convert.ToInt32(values[SPFields[SPFieldNames.HandoutId].InternalName]);
            }

            return true;
        }

        public override IDictionary<string, string> GetUserFieldValues()
        {
            return new Dictionary<string, string>
                   {
                       { SPFields[SPFieldNames.ProgramId].InternalName, ProgramId },
                       { SPFields[SPFieldNames.HandoutId].InternalName, HandoutId.ToString() }
                   };
        }

        public override string[] ParseFileName(string fileNameToParse)
        {
            string[] fileNameParts = base.ParseFileName(fileNameToParse);

            ProgramId = fileNameParts[1];

            if (!int.TryParse(fileNameParts[2], out int tempHandoutId))
            {
                ThrowFileNameExceptionInvalidType(fileNameToParse, SPFieldNames.HandoutId, "Integer");
            }

            HandoutId = tempHandoutId;

            return fileNameParts;
        }
    }
}
