using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

using MEI.SPDocuments.Data;
using MEI.SPDocuments.TypeCodes;

namespace MEI.SPDocuments.Document
{
    [DocumentInfo(SPDocumentType.TrainingWaiver, "TrainingWaiver", "TW")]
    public class TrainingWaiver
        : SPDocumentBase, ISearchProgram, ISearchSpeaker
    {
        internal TrainingWaiver(IRepository repository, IDbUtilities dbUtilities, DocumentTypeInfo documentTypeInfo)
            : base(repository, dbUtilities, documentTypeInfo)
        { }

        public TrainingWaiver WithValues(string programId, int? slideKitId)
        {
            ProgramId = programId;
            SlideKitId = slideKitId;

            return this;
        }

        public override DocumentYear DocumentYear => ExtractDocumentYear(ProgramId);

        public override string SearchFieldValue => ProgramId;

        [SPFieldInfo(SPFieldNames.SlideKitId, "SlideKitID", SPFieldType.Text, 1)]
        public int? SlideKitId { get; private set; }

        [SPFieldInfo(SPFieldNames.ProgramId, "ProgramID", SPFieldType.Text, 0)]
        public string ProgramId { get; private set; }

        public override string FileName => MakeFileName(ProgramId, SlideKitId);

        public override bool IsValid
        {
            get
            {
                bool baseValid = base.IsValid;

                if (!SlideKitId.HasValue)
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

        public override IList<string> AllowedFileTypes =>
            new List<string>
            {
                "pdf",
                "ppt",
                "pptx",
                "doc",
                "docx"
            };

        public override string UniqueIdentifiers => "ProgramId;SlideKitId";

        public override string UniqueValues => string.Format("{0};{1}", ProgramId, SlideKitId);

        public ISearchExpressionGroup GetSearchExpressionGroupByProgram(Company company, DocumentYear year, string programId)
        {
            return new SearchExpressionGroup(this,
                SPFieldNames.ProgramId,
                CamlComparison.Equal,
                DbUtilities.FromDbValue<string>(programId));
        }

        public ISearchExpressionGroup GetSearchExpressionGroupBySpeaker(Company company, DocumentYear year, int speakerCounter)
        {
            DataTable dt = Repository.GetSlideKitIdBySpeakerCounter(company, year, speakerCounter);

            var seg = new SearchExpressionGroup(this);

            foreach (DataRow dr in dt.Rows)
            {
                seg.AddExpression(SPFieldNames.SlideKitId, CamlComparison.Equal, DbUtilities.FromDbValue<int>(dr["SlideKitId"]));
            }

            return seg;
        }

        public override bool ValidateFields()
        {
            if (!IsValid)
            {
                return false;
            }

            if (Repository.GetProgramIdsByProgramId(Company, DocumentYear, ProgramId).Rows.Count <= 0)
            {
                return false;
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

            ProgramId = Convert.ToString(objects[0]);
            if (objects[0] != null)
            {
                SlideKitId = Convert.ToInt32(objects[1]);
            }

            Contents = (byte[])objects[2];
            FileExtension = objects[3].ToString();
            Company = (Company)objects[4];

            return IsValid;
        }

        public override bool AbstractSetup(Hashtable values)
        {
            if (values.ContainsKey(SPFields[SPFieldNames.SlideKitId].InternalName))
            {
                SlideKitId = Convert.ToInt32(values[SPFields[SPFieldNames.SlideKitId].InternalName]);
            }

            if (values.ContainsKey(SPFields[SPFieldNames.ProgramId].InternalName))
            {
                ProgramId = Convert.ToString(values[SPFields[SPFieldNames.ProgramId].InternalName]);
            }

            return true;
        }

        public override IDictionary<string, string> GetUserFieldValues()
        {
            return new Dictionary<string, string>
                   {
                       { SPFields[SPFieldNames.ProgramId].InternalName, ProgramId },
                       { SPFields[SPFieldNames.SlideKitId].InternalName, SlideKitId.ToString() }
                   };
        }

        public override string[] ParseFileName(string fileNameToParse)
        {
            string[] fileNameParts = base.ParseFileName(fileNameToParse);

            ProgramId = fileNameParts[1];

            if (!int.TryParse(fileNameParts[2], out int temp))
            {
                ThrowFileNameExceptionInvalidType(fileNameToParse, SPFieldNames.SlideKitId, "Integer");
            }

            SlideKitId = temp;

            return fileNameParts;
        }
    }
}
