using System;
using System.Collections;
using System.Collections.Generic;

using MEI.SPDocuments.Data;
using MEI.SPDocuments.TypeCodes;

namespace MEI.SPDocuments.Document
{
    [DocumentInfo(SPDocumentType.RSVPInvite, "RSVPInvite", "RNVT")]
    public class RSVPInvite
        : SPDocumentBase, ISearchProgram, ISearchYear
    {
        internal RSVPInvite(IRepository repository, IDbUtilities dbUtilities, DocumentTypeInfo documentTypeInfo)
            : base(repository, dbUtilities, documentTypeInfo)
        { }

        public RSVPInvite WithValues(string programId, int? inviteId)
        {
            InviteId = inviteId;
            ProgramId = programId;

            return this;
        }

        public override DocumentYear DocumentYear => ExtractDocumentYear(ProgramId);

        public override string SearchFieldValue => ProgramId;

        public override string ParsableFileName => MakeFileName(ProgramId, InviteId);

        [SPFieldInfo(SPFieldNames.ProgramId, "Program_x0020_ID", SPFieldType.Text, "Program ID", 0)]
        public string ProgramId { get; private set; }

        [SPFieldInfo(SPFieldNames.InviteId, "InviteID", SPFieldType.Text, 1)]
        public int? InviteId { get; private set; }

        public override string FileName => MakeFileName(ProgramId);

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

                return baseValid;
            }
        }

        public override string UniqueIdentifiers => "ProgramId";

        public override string UniqueValues => string.Format("{0}", ProgramId);

        public override IList<string> AllowedFileTypes =>
            new List<string>
            {
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

            if (values.ContainsKey(SPFields[SPFieldNames.InviteId].InternalName))
            {
                InviteId = Convert.ToInt32(values[SPFields[SPFieldNames.InviteId].InternalName]);
            }

            return true;
        }

        public override bool ValidateFields()
        {
            if (!IsValid)
            {
                return false;
            }

            if (Company == Company.AbbottStructuralHeart)
            {
                if (Repository.GetProgramIdsByProgramId(Company, DocumentYear.Year2018, ProgramId).Rows.Count <= 0)
                {
                    ThrowFileNameExceptionNoDBMatch(SPFieldNames.ProgramId, ProgramId);
                }

                if (InviteId != null && Repository.GetInviteIdsByInviteId(Company, DocumentYear.Year2018, InviteId.Value).Rows.Count <= 0)
                {
                    ThrowFileNameExceptionNoDBMatch(SPFieldNames.InviteId, InviteId.Value.ToString());
                }
            }
            else
            {
                if (Repository.GetProgramIdsByProgramId(Company, DocumentYear, ProgramId).Rows.Count <= 0)
                {
                    ThrowFileNameExceptionNoDBMatch(SPFieldNames.ProgramId, ProgramId);
                }

                if (InviteId != null && Repository.GetInviteIdsByInviteId(Company, DocumentYear, InviteId.Value).Rows.Count <= 0)
                {
                    ThrowFileNameExceptionNoDBMatch(SPFieldNames.InviteId, InviteId.Value.ToString());
                }
            }

            return true;
        }

        public override IDictionary<string, string> GetUserFieldValues()
        {
            return new Dictionary<string, string>
                   {
                       { SPFields[SPFieldNames.ProgramId].InternalName, ProgramId },
                       { SPFields[SPFieldNames.InviteId].InternalName, InviteId.ToString() }
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

            return fileNameParts;
        }
    }
}
