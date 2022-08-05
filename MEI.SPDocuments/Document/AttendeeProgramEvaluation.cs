using System;
using System.Collections;
using System.Collections.Generic;

using MEI.SPDocuments.Data;
using MEI.SPDocuments.TypeCodes;

namespace MEI.SPDocuments.Document
{
    [DocumentInfo(SPDocumentType.APE, "AttendeeProgramEvaluation", "APE", "APE", "Attendee Program Evaluation")]
    public class AttendeeProgramEvaluation
        : SPDocumentBase, ISearchProgram, ISearchYear
    {
        private readonly IPdfTools _pdfTools;

        internal AttendeeProgramEvaluation(IRepository repository,
                                           IDbUtilities dbUtilities,
                                           DocumentTypeInfo documentTypeInfo,
                                           IPdfTools pdfTools)
            : base(repository, dbUtilities, documentTypeInfo)
        {
            _pdfTools = pdfTools;
        }

        public AttendeeProgramEvaluation WithValues(string programId)
        {
            ProgramId = programId;

            return this;
        }

        public override DocumentYear DocumentYear => ExtractDocumentYear(ProgramId);

        [SPFieldInfo(SPFieldNames.ProgramId, "ProgramID", SPFieldType.Text, 0)]
        public string ProgramId { get; private set; }

        [SPFieldInfo(SPFieldNames.SequenceNumber, "SequenceNum", SPFieldType.Text, 1)]
        public int? SequenceNumber { get; set; }

        public override string FileName => MakeFileName(ProgramId, SequenceNumber);

        public override bool IsValid
        {
            get
            {
                bool baseValid = base.IsValid;

                if (string.IsNullOrEmpty(ProgramId))
                {
                    return false;

                    //ElseIf Not SequenceNum.HasValue Then
                    //    Return False
                }

                return baseValid;
            }
        }

        public override string UniqueIdentifiers => "ProgramId;SequenceNumber";

        public override string UniqueValues => string.Format("{0};{1}", ProgramId, SequenceNumber);

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
            SequenceNumber = Convert.ToInt32(objects[1]);
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

            if (values.ContainsKey(SPFields[SPFieldNames.SequenceNumber].InternalName))
            {
                SequenceNumber = Convert.ToInt32(values[SPFields[SPFieldNames.SequenceNumber].InternalName]);
            }

            return true;
        }

        public override IDictionary<string, string> GetUserFieldValues()
        {
            return new Dictionary<string, string>
                   {
                       { SPFields[SPFieldNames.ProgramId].InternalName, ProgramId },
                       { SPFields[SPFieldNames.SequenceNumber].InternalName, SequenceNumber.ToString() }
                   };
        }

        public override string[] ParseFileName(string fileNameToParse)
        {
            string[] fileNameParts = base.ParseFileName(fileNameToParse);

            ProgramId = fileNameParts[1];

            if (!string.IsNullOrEmpty(fileNameParts[2]))
            {
                if (int.TryParse(fileNameParts[2], out int tempSequenceNumber))
                {
                    SequenceNumber = tempSequenceNumber;
                }
            }

            return fileNameParts;
        }

        public override void ActionBeforeUpload(IDocumentBroker documentBroker)
        {
            if (((Company == Company.Abbott) || (Company == Company.ExactSciences) || (Company == Company.Alkermes)
                 || (Company == Company.AbbottNutrition) || (Company == Company.Kowa) || (Company == Company.LabDevelopment))
                && DocumentYear.Compare(DocumentYear.Year2012, ">="))
            {
                int pageCount = _pdfTools.GetPageCount(Contents);

                if (!SequenceNumber.HasValue)
                {
                    SequenceNumber = Repository.GetSequenceNumberForAttendeeProgramEvaluation(Company, DocumentYear, ProgramId, pageCount);
                }
            }
            else if (Company == Company.LabDevelopment)
            {
                SequenceNumber = 0;
            }
        }
    }
}
