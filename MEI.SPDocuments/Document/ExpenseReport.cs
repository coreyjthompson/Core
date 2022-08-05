using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using MEI.SPDocuments.Data;
using MEI.SPDocuments.SPActionResult;
using MEI.SPDocuments.TypeCodes;

namespace MEI.SPDocuments.Document
{
    /// <remarks>NOTE: ExpenseReportW9 is being saved as W9</remarks>
    [DocumentInfo(SPDocumentType.ExpenseReport, "ExpenseReport", "ER", "ExpenseReport", "Expense Report")]
    public class ExpenseReport
        : SPDocumentBase, ISearchSpeaker, ISearchProgram, ISearchYear
    {
        private readonly IEmailer _emailer;

        internal ExpenseReport(IRepository repository, IDbUtilities dbUtilities, DocumentTypeInfo documentTypeInfo, IEmailer emailer)
            : base(repository, dbUtilities, documentTypeInfo)
        {
            _emailer = emailer;
        }

        public ExpenseReport WithValues(string programId, int? speakerCounter)
        {
            ProgramId = programId;
            SpeakerCounter = speakerCounter;

            return this;
        }

        public override DocumentYear DocumentYear => ExtractDocumentYear(ProgramId);

        public override string SearchFieldValue => ProgramId;

        [SPFieldInfo(SPFieldNames.ProgramId, "Program_x0020_ID", SPFieldType.Text, "Program ID", 0)]
        public string ProgramId { get; private set; }

        [SPFieldInfo(SPFieldNames.SpeakerCounter, "SpeakerCounter", SPFieldType.Text, 1)]
        public int? SpeakerCounter { get; private set; }

        public override string FileName => MakeFileName(ProgramId, SpeakerCounter);

        public override bool IsValid
        {
            get
            {
                bool baseValid = base.IsValid;

                if (string.IsNullOrEmpty(ProgramId))
                {
                    return false;
                }

                if (!SpeakerCounter.HasValue)
                {
                    return false;
                }

                return baseValid;
            }
        }

        public override string UniqueIdentifiers => "ProgramId;SpeakerCounter";

        public override string UniqueValues => string.Format("{0};{1}", ProgramId, SpeakerCounter);

        public ISearchExpressionGroup GetSearchExpressionGroupByProgram(Company company, DocumentYear year, string programId)
        {
            return new SearchExpressionGroup(this, SPFieldNames.ProgramId, CamlComparison.Equal, programId);
        }

        public ISearchExpressionGroup GetSearchExpressionGroupBySpeaker(Company company, DocumentYear year, int speakerCounter)
        {
            return new SearchExpressionGroup(this, SPFieldNames.SpeakerCounter, CamlComparison.Equal, speakerCounter);
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

            if (SpeakerCounter != null && Repository.GetSpeakerCountersBySpeakerCounter(Company, DocumentYear, SpeakerCounter.Value).Rows.Count <= 0)
            {
                ThrowFileNameExceptionNoDBMatch(SPFieldNames.SpeakerCounter, SpeakerCounter.Value.ToString());
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
            SpeakerCounter = Convert.ToInt32(objects[1]);
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

            if (values.ContainsKey(SPFields[SPFieldNames.SpeakerCounter].InternalName))
            {
                SpeakerCounter = Convert.ToInt32(values[SPFields[SPFieldNames.SpeakerCounter].InternalName]);
            }

            return true;
        }

        public override IDictionary<string, string> GetUserFieldValues()
        {
            return new Dictionary<string, string>
                   {
                       { SPFields[SPFieldNames.ProgramId].InternalName, ProgramId },
                       { SPFields[SPFieldNames.SpeakerCounter].InternalName, SpeakerCounter.ToString() }
                   };
        }

        public override string[] ParseFileName(string fileNameToParse)
        {
            string[] fileNameParts = base.ParseFileName(fileNameToParse);

            ProgramId = fileNameParts[1];

            if (!int.TryParse(fileNameParts[2], out int tempSpeakerCounter))
            {
                ThrowFileNameExceptionInvalidType(fileNameToParse, SPFieldNames.SpeakerCounter, "Integer");
            }

            SpeakerCounter = tempSpeakerCounter;

            return fileNameParts;
        }

        public override int ActionAfterUpload(UploadResult result)
        {
            if ((Company == Company.Quest) && (result.Status == SPActionStatus.Success))
            {
                var sb = new StringBuilder();
                sb.AppendLine("A new expense report has been uploaded to SharePoint.<br />");
                sb.AppendLine(string.Format("ProgramID: {0}<br />", ProgramId));
                sb.AppendLine(string.Format("Speaker Counter: {0}<br />", SpeakerCounter));
                sb.AppendLine(string.Format("Document Location: https://development.meintl.com/MEIIntranet/Documents/DocumentSearch.aspx?GetFile=ER|{0};{1}&Client={2}&Year={3}",
                    ProgramId,
                    SpeakerCounter,
                    Company.ToDisplayNameLong(),
                    DocumentYear.ToDisplayNameLong()));

                _emailer.SendEmail("Expense Report Added to SharePoint", sb.ToString(), new[] { "questdiagnostics@meintl.com" }, true);
            }

            return 0;
        }
    }
}
