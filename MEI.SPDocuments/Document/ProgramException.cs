using System;
using System.Collections;
using System.Collections.Generic;

using MEI.SPDocuments.Data;
using MEI.SPDocuments.TypeCodes;

namespace MEI.SPDocuments.Document
{
    [DocumentInfo(SPDocumentType.ProgramException, "ProgramException", "PE", "PE", "ProgramException", "ProgramException")]
    public class ProgramException
        : SPDocumentBase, ISearchProgram, ISearchYear
    {
        internal ProgramException(IRepository repository, IDbUtilities dbUtilities, DocumentTypeInfo documentTypeInfo)
            : base(repository, dbUtilities, documentTypeInfo)
        { }

        public ProgramException WithValues(string programId, ProgramExceptionType exceptionTypeCode)
        {
            ProgramId = programId;
            ProgramExceptionType = exceptionTypeCode;

            return this;
        }

        public override DocumentYear DocumentYear => ExtractDocumentYear(ProgramId);

        public override string SearchFieldValue => ProgramId;

        [SPFieldInfo(SPFieldNames.ProgramId, "ProgramID", SPFieldType.Text, 0)]
        public string ProgramId { get; private set; }

        [SPFieldInfo(SPFieldNames.ExceptionType, "ExceptionType", SPFieldType.Choice, 1)]
        public ProgramExceptionType ProgramExceptionType { get; private set; }

        public override string FileName => MakeFileName(ProgramExceptionType.ToDisplayNameShort(), ProgramId);

        public override bool IsValid
        {
            get
            {
                bool baseValid = base.IsValid;

                if (ProgramExceptionType == ProgramExceptionType.Undefined)
                {
                    return false;
                }

                if (string.IsNullOrEmpty(ProgramId))
                {
                    return false;
                }

                return baseValid;
            }
        }

        public override string UniqueIdentifiers => "SpeakerCounter;ProgramId";

        public override string UniqueValues => string.Format("{0};{1}", ProgramExceptionType, ProgramId);

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

            ProgramExceptionType = ProgramExceptionType.Undefined;

            if (objects[0].ToString() == "Location")
            {
                ProgramExceptionType = ProgramExceptionType.L;
            }
            else if (objects[0].ToString() == "Speaker")
            {
                ProgramExceptionType = ProgramExceptionType.S;
            }

            ProgramId = objects[1].ToString();

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

            if (values.ContainsKey(SPFields[SPFieldNames.ExceptionType].InternalName))
            {
                ProgramExceptionType = (ProgramExceptionType)Enum.Parse(typeof(ProgramExceptionType),
                    values[SPFields[SPFieldNames.ExceptionType].InternalName].ToString());
            }

            return true;
        }

        public override IDictionary<string, string> GetUserFieldValues()
        {
            return new Dictionary<string, string>
                   {
                       { SPFields[SPFieldNames.ProgramId].InternalName, ProgramId },
                       { SPFields[SPFieldNames.ExceptionType].InternalName, ProgramExceptionType.ToDisplayNameShort() }
                   };
        }

        public override string[] ParseFileName(string fileNameToParse)
        {
            // file name format: PE__<ExceptionTypeCode>__<ProgramId>_<Client><Year>.pdf - example: PE_Location/Speaker_DD309-DM07-19__Abbott2019.pdf
            string[] fileNameParts = base.ParseFileName(fileNameToParse);

            ProgramExceptionType = fileNameParts[1].ToProgramExceptionType();

            if (ProgramExceptionType == ProgramExceptionType.Undefined)
            {
                ThrowFileNameExceptionInvalidType(fileNameToParse, SPFieldNames.ExceptionType, "ProgramExpceptionType");
            }

            ProgramId = fileNameParts[2];

            return fileNameParts;
        }
    }
}
