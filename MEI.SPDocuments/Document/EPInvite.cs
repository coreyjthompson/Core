using System;
using System.Collections;
using System.Collections.Generic;

using MEI.SPDocuments.Data;
using MEI.SPDocuments.TypeCodes;

namespace MEI.SPDocuments.Document
{
    [DocumentInfo(SPDocumentType.EPInvite, "EPInvite", "EPNVT")]
    public class EPInvite
        : SPDocumentBase
    {
        internal EPInvite(IRepository repository, IDbUtilities dbUtilities, DocumentTypeInfo documentTypeInfo)
            : base(repository, dbUtilities, documentTypeInfo)
        { }

        public EPInvite WithValues(int? inviteId, EPassStatus statusTypeCode)
        {
            InviteId = inviteId;
            StatusTypeCode = statusTypeCode;

            return this;
        }

        public override DocumentYear DocumentYear => DocumentYear.Undefined;

        [SPFieldInfo(SPFieldNames.InviteId, "InviteID", SPFieldType.Text, 0)]
        public int? InviteId { get; private set; }

        [SPFieldInfo(SPFieldNames.StatusCode, "StatusCode", SPFieldType.Text, 1)]
        public EPassStatus StatusTypeCode { get; private set; }

        public override string FileName => MakeFileName(InviteId, StatusTypeCode.ToDisplayNameShort());

        public override bool IsValid
        {
            get
            {
                bool baseValid = base.IsValid;

                if (!InviteId.HasValue)
                {
                    return false;
                }

                if (StatusTypeCode == EPassStatus.Undefined)
                {
                    return false;
                }

                return baseValid;
            }
        }

        public override string UniqueIdentifiers => "InviteId;Status";

        public override string UniqueValues => string.Format("{0};{1}", InviteId, StatusTypeCode.ToDisplayNameLong());

        public override bool ValidateFields()
        {
            if (!IsValid)
            {
                return false;
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

            InviteId = Convert.ToInt32(objects[0]);
            StatusTypeCode = objects[1].ToString().ToEPassStatus();
            Contents = (byte[])objects[2];
            FileExtension = objects[3].ToString();
            Company = (Company)objects[4];

            return IsValid;
        }

        public override bool AbstractSetup(Hashtable values)
        {
            if (values.ContainsKey(SPFields[SPFieldNames.InviteId].InternalName))
            {
                InviteId = Convert.ToInt32(values[SPFields[SPFieldNames.InviteId].InternalName]);
            }

            if (values.ContainsKey(SPFields[SPFieldNames.StatusCode].InternalName))
            {
                StatusTypeCode = ((string)values[SPFields[SPFieldNames.StatusCode].InternalName]).ToEPassStatus();
            }

            return true;
        }

        public override IDictionary<string, string> GetUserFieldValues()
        {
            return new Dictionary<string, string>
                   {
                       { SPFields[SPFieldNames.InviteId].InternalName, InviteId.ToString() },
                       { SPFields[SPFieldNames.StatusCode].InternalName, StatusTypeCode.ToDisplayNameShort() },
                   };
        }

        public override string[] ParseFileName(string fileNameToParse)
        {
            string[] fileNameParts = base.ParseFileName(fileNameToParse);

            if (!int.TryParse(fileNameParts[1], out int tempInviteId))
            {
                ThrowFileNameExceptionInvalidType(fileNameToParse, SPFieldNames.InviteId, "Integer");
            }

            InviteId = tempInviteId;

            StatusTypeCode = fileNameParts[2].ToEPassStatus();
            if (StatusTypeCode == EPassStatus.Undefined)
            {
                ThrowFileNameExceptionInvalidType(fileNameToParse, SPFieldNames.StatusCode, "EPassStatusCode");
            }

            return fileNameParts;
        }
    }
}
