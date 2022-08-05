using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

using MEI.SPDocuments.Data;
using MEI.SPDocuments.SPActionResult;
using MEI.SPDocuments.TypeCodes;

namespace MEI.SPDocuments.Document
{
    [DocumentInfo(SPDocumentType.SampleInvite, "SampleInvite", "SNVT")]
    public class SampleInvite
        : SPDocumentBase, ISearchProgram, ISearchYear
    {
        internal SampleInvite(IRepository repository, IDbUtilities dbUtilities, DocumentTypeInfo documentTypeInfo)
            : base(repository, dbUtilities, documentTypeInfo)
        { }

        public SampleInvite WithValues(string programId, int? inviteId, string versionNumber, DateTime uploadDate)
        {
            InviteId = inviteId;
            ProgramId = programId;
            VersionNumber = versionNumber;
            UploadDate = uploadDate;

            return this;
        }

        public override DocumentYear DocumentYear => ExtractDocumentYear(ProgramId);

        public override string SearchFieldValue => ProgramId;

        public override string ParsableFileName => MakeFileName(ProgramId, InviteId, VersionNumber, UploadDate.ToString("yyyyMMddHHmmss"));

        [SPFieldInfo(SPFieldNames.ProgramId, "Program_x0020_ID", SPFieldType.Text, 0)]
        public string ProgramId { get; private set; }

        [SPFieldInfo(SPFieldNames.InviteId, "InviteID", SPFieldType.Text, 1)]
        public int? InviteId { get; private set; }

        [SPFieldInfo(SPFieldNames.UploadDate, "UploadDate", SPFieldType.DateTime, 3)]
        public DateTime UploadDate { get; private set; }

        [SPFieldInfo(SPFieldNames.VersionNumber, "VersionNum", SPFieldType.Text, 2)]
        public string VersionNumber { get; private set; }

        public override string FileName => MakeFileName(ProgramId, InviteId);

        public override bool IsValid
        {
            get
            {
                bool baseValid = base.IsValid;

                if (string.IsNullOrEmpty(ProgramId))
                {
                    return false;
                }

                if (!InviteId.HasValue)
                {
                    return false;
                }

                if ((Company == Company.Abbott) | (Company == Company.ExactSciences))
                {
                    if (string.IsNullOrEmpty(VersionNumber))
                    {
                        return false;
                    }
                }

                return baseValid;
            }
        }

        public override string UniqueIdentifiers => "ProgramId;InviteId;VersionNum;UploadDate";

        public override string UniqueValues =>
            string.Format("{0};{1};{2};{3:yyyyMMddHHmmss}", ProgramId, InviteId, VersionNumber, UploadDate);

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
                ThrowFileNameExceptionNoDBMatch(SPFieldNames.InviteId, InviteId.Value.ToString());
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
            InviteId = Convert.ToInt32(objects[1]);
            VersionNumber = objects[3].ToString();
            UploadDate = (DateTime)objects[4];

            if (objects[4] is byte[] contentsValue)
            {
                Contents = contentsValue;
            }

            Contents = (byte[])objects[4];
            FileExtension = objects[5].ToString();
            Company = (Company)objects[6];

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

            if (values.Contains(SPFields[SPFieldNames.VersionNumber].InternalName))
            {
                VersionNumber = (string)values[SPFields[SPFieldNames.VersionNumber].InternalName];
            }

            if (values.Contains(SPFields[SPFieldNames.UploadDate].InternalName))
            {
                UploadDate = (DateTime)values[SPFields[SPFieldNames.UploadDate].InternalName];
            }

            return true;
        }

        public override IDictionary<string, string> GetUserFieldValues()
        {
            return new Dictionary<string, string>
                   {
                       { SPFields[SPFieldNames.ProgramId].InternalName, ProgramId },
                       { SPFields[SPFieldNames.InviteId].InternalName, InviteId.ToString() },
                       { SPFields[SPFieldNames.VersionNumber].InternalName, VersionNumber },
                       { SPFields[SPFieldNames.UploadDate].InternalName, UploadDate.ToString("yyyyMMddHHmmss") }
                   };
        }

        public override string[] ParseFileName(string fileNameToParse)
        {
            string[] fileNameParts = base.ParseFileName(fileNameToParse);

            ProgramId = fileNameParts[1];

            if (!int.TryParse(fileNameParts[2], out int tempInviteId))
            {
                ThrowFileNameExceptionInvalidType(fileNameToParse, SPFieldNames.InviteId, "Integer");
            }

            InviteId = tempInviteId;

            if ((Company == Company.Abbott) | (Company == Company.ExactSciences))
            {
                VersionNumber = fileNameParts[3];
            }
            else
            {
                VersionNumber = "";
            }

            UploadDate = DateTime.ParseExact(fileNameParts[4], "yyyyMMddHHmmss", CultureInfo.InvariantCulture);

            return fileNameParts;
        }

        public override int ActionAfterUpload(UploadResult result)
        {
            if (result.Status == SPActionStatus.Success)
            {
                Repository.SendInviteApprovalEmail(Company, DocumentYear, ProgramId);
            }

            return 0;
        }

        public override WatermarkProfile GetWaterMarkProfile(string connectionString)
        {
            return new WatermarkProfile("WatermarkDocumentCentered", null, null, 100, WatermarkTextDrawStyle.Solid, "Sample");
        }
    }
}
