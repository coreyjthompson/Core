using System;
using System.Collections;
using System.Collections.Generic;

using MEI.SPDocuments.Data;
using MEI.SPDocuments.TypeCodes;

namespace MEI.SPDocuments.Document
{
    [DocumentInfo(SPDocumentType.MaterialLetter, "MaterialLetter", "ML", "ML", "Material Letter")]
    public class MaterialLetter
        : SPDocumentBase, ISearchProgram, ISearchYear
    {
        internal MaterialLetter(IRepository repository, IDbUtilities dbUtilities, DocumentTypeInfo documentTypeInfo)
            : base(repository, dbUtilities, documentTypeInfo)
        { }

        public MaterialLetter WithValues(string programId, string letterType, int? inviteId)
        {
            ProgramId = programId;
            LetterType = letterType;
            InviteId = inviteId;

            return this;
        }

        public override DocumentYear DocumentYear => ExtractDocumentYear(ProgramId);

        [SPFieldInfo(SPFieldNames.ProgramId, "ProgramId", SPFieldType.Text, 1)]
        public string ProgramId { get; private set; }

        [SPFieldInfo(SPFieldNames.LetterType, "LetterType", SPFieldType.Text, 0)]
        public string LetterType { get; private set; }

        [SPFieldInfo(SPFieldNames.InviteId, "InviteId", SPFieldType.Text, 3)]
        public int? InviteId { get; private set; }

        public override string FileName => MakeFileName(LetterType, ProgramId, InviteId);

        public override bool IsValid
        {
            get
            {
                bool baseValid = base.IsValid;

                if (string.IsNullOrEmpty(ProgramId))
                {
                    return false;
                }

                if (string.IsNullOrEmpty(LetterType))
                {
                    return false;
                }

                if (!InviteId.HasValue)
                {
                    return false;
                }

                return baseValid;
            }
        }

        public override string UniqueIdentifiers => "LetterType;ProgramId;InviteId";

        public override string UniqueValues => string.Format("{0};{1};{2}", LetterType, ProgramId, InviteId);

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

            if (InviteId != null && Repository.GetInviteIdsByInviteId(Company, DocumentYear, InviteId.Value).Rows.Count <= 0)
            {
                if (InviteId != 1)
                {
                    ThrowFileNameExceptionNoDBMatch(SPFieldNames.InviteId, InviteId.Value.ToString());
                }
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

            LetterType = objects[0].ToString();
            ProgramId = objects[1].ToString();
            InviteId = Convert.ToInt32(objects[2]);
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

            if (values.ContainsKey(SPFields[SPFieldNames.InviteId].InternalName))
            {
                InviteId = Convert.ToInt32(values[SPFields[SPFieldNames.InviteId].InternalName]);
            }

            if (values.ContainsKey(SPFields[SPFieldNames.LetterType].InternalName))
            {
                LetterType = (string)values[SPFields[SPFieldNames.LetterType].InternalName];
            }

            return true;
        }

        public override IDictionary<string, string> GetUserFieldValues()
        {
            return new Dictionary<string, string>
                   {
                       { SPFields[SPFieldNames.LetterType].InternalName, LetterType },
                       { SPFields[SPFieldNames.ProgramId].InternalName, ProgramId },
                       { SPFields[SPFieldNames.InviteId].InternalName, InviteId.ToString() }
                   };
        }

        public override string[] ParseFileName(string fileNameToParse)
        {
            string[] fileNameParts = base.ParseFileName(fileNameToParse);

            LetterType = fileNameParts[1];
            ProgramId = fileNameParts[2];

            if (!int.TryParse(fileNameParts[3], out int tempInviteId))
            {
                ThrowFileNameExceptionInvalidType(fileNameToParse, SPFieldNames.InviteId, "Integer");
            }

            InviteId = tempInviteId;

            return fileNameParts;
        }
    }
}
