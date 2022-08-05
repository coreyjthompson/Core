using System;
using System.Collections;
using System.Collections.Generic;

using MEI.SPDocuments.Data;
using MEI.SPDocuments.TypeCodes;

namespace MEI.SPDocuments.Document
{
    [DocumentInfo(SPDocumentType.Invite, "Invite", "NVT")]
    public class Invite
        : SPDocumentBase, ISearchProgram, ISearchYear
    {
        internal Invite(IRepository repository, IDbUtilities dbUtilities, DocumentTypeInfo documentTypeInfo)
            : base(repository, dbUtilities, documentTypeInfo)
        { }

        public Invite WithValues(string programId, int? inviteId, InviteType inviteTypeCode, string versionNumber)
        {
            InviteId = inviteId;
            ProgramId = programId;
            InviteTypeCode = inviteTypeCode;
            VersionNumber = versionNumber;

            return this;
        }

        public override DocumentYear DocumentYear => ExtractDocumentYear(ProgramId);

        public override string SearchFieldValue => ProgramId;

        public override string ParsableFileName => MakeFileName(ProgramId, InviteId, InviteTypeCode.ToDisplayNameLong(), VersionNumber);

        [SPFieldInfo(SPFieldNames.ProgramId, "Program_x0020_ID", SPFieldType.Text, 0)]
        public string ProgramId { get; private set; }

        [SPFieldInfo(SPFieldNames.InviteId, "InviteID", SPFieldType.Text, 1)]
        public int? InviteId { get; private set; }

        [SPFieldInfo(SPFieldNames.InviteType, "InviteTypeCode", SPFieldType.Choice, 2)]
        public InviteType InviteTypeCode { get; private set; }

        [SPFieldInfo(SPFieldNames.VersionNumber, "VersionNum", SPFieldType.Text, 3)]
        public string VersionNumber { get; private set; }

        public override string FileName => MakeFileName(ProgramId, InviteTypeCode.ToDisplayNameShort());

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
                    if (InviteTypeCode == InviteType.Undefined)
                    {
                        return false;
                    }

                    if (string.IsNullOrEmpty(VersionNumber))
                    {
                        return false;
                    }
                }

                return baseValid;
            }
        }

        public override string UniqueIdentifiers => "ProgramId;InviteTypeCode";

        public override string UniqueValues => string.Format("{0};{1}", ProgramId, InviteTypeCode);

        public override IList<string> AllowedFileTypes =>
            new List<string>
            {
                "pdf",
                "png",
                "jpg"
            };

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

            //If Company = CompanyCode.Abbott Then
            //    If InviteTypeCode = Type.InviteTypeCode.Undefined Then
            //        Return False
            //    End If

            //    If Not Validator.ValidateCardStockVersion(Company, DocumentYear, ProgramId, VersionNum) Then
            //        Return False
            //    End If
            //End If
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
            InviteTypeCode = objects[2].ToString().ToInviteType();
            VersionNumber = objects[3].ToString();
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

            if (values.ContainsKey(SPFields[SPFieldNames.InviteType].InternalName))
            {
                InviteTypeCode = ((string)values[SPFields[SPFieldNames.InviteType].InternalName]).ToInviteType();
            }

            if (values.Contains(SPFields[SPFieldNames.InviteType].InternalName))
            {
                VersionNumber = (string)values[SPFields[SPFieldNames.VersionNumber].InternalName];
            }

            return true;
        }

        public override IDictionary<string, string> GetUserFieldValues()
        {
            return new Dictionary<string, string>
                   {
                       { SPFields[SPFieldNames.ProgramId].InternalName, ProgramId },
                       { SPFields[SPFieldNames.InviteId].InternalName, InviteId.ToString() },
                       { SPFields[SPFieldNames.InviteType].InternalName, InviteTypeCode.ToDisplayNameShort() },
                       { SPFields[SPFieldNames.VersionNumber].InternalName, VersionNumber }
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
                InviteTypeCode = fileNameParts[3].ToInviteType();

                if (InviteTypeCode == InviteType.Undefined)
                {
                    ThrowFileNameExceptionInvalidType(fileNameToParse, SPFieldNames.InviteType, "InviteTypeCode");
                }

                VersionNumber = fileNameParts[4];
            }
            else
            {
                InviteTypeCode = InviteType.None;
                VersionNumber = "";
            }

            return fileNameParts;
        }
    }
}
