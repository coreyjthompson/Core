using System;
using System.Collections;
using System.Collections.Generic;

using MEI.SPDocuments.Data;
using MEI.SPDocuments.TypeCodes;

namespace MEI.SPDocuments.Document
{
    [DocumentInfo(SPDocumentType.PRMApproval, "PRMApproval", "PRMA", "PRMA", "PRM Approval")]
    public class PRMApproval
        : SPDocumentBase
    {
        internal PRMApproval(IRepository repository, IDbUtilities dbUtilities, DocumentTypeInfo documentTypeInfo)
            : base(repository, dbUtilities, documentTypeInfo)
        { }

        public PRMApproval WithValues(int? adhocSlideKitId)
        {
            AdHocSlideKitId = adhocSlideKitId;

            return this;
        }

        public override DocumentYear DocumentYear => DocumentYear.Undefined;

        [SPFieldInfo(SPFieldNames.AdHocSlideKitId, "AdhocSlidekitId", SPFieldType.Text, 0)]
        public int? AdHocSlideKitId { get; private set; }

        public override string FileName => MakeFileName(AdHocSlideKitId);

        public override bool IsValid
        {
            get
            {
                bool baseValid = base.IsValid;

                if (!AdHocSlideKitId.HasValue)
                {
                    return false;
                }

                return baseValid;
            }
        }

        public override string UniqueIdentifiers => "AdHocSlideKitId";

        public override string UniqueValues => AdHocSlideKitId.ToString();

        public override bool ValidateFields()
        {
            if (!IsValid)
            {
                return false;
            }

            //TODO: Make AdHoc Slide Kit ID Validator

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

            if (objects[0] != null)
            {
                AdHocSlideKitId = Convert.ToInt32(objects[0]);
            }

            Contents = (byte[])objects[1];
            FileExtension = objects[2].ToString();
            Company = (Company)objects[3];

            return IsValid;
        }

        public override bool AbstractSetup(Hashtable values)
        {
            if (values.ContainsKey(SPFields[SPFieldNames.AdHocSlideKitId].InternalName))
            {
                AdHocSlideKitId = Convert.ToInt32(values[SPFields[SPFieldNames.AdHocSlideKitId].InternalName]);
            }

            return true;
        }

        public override IDictionary<string, string> GetUserFieldValues()
        {
            return new Dictionary<string, string>
                   {
                       { SPFields[SPFieldNames.AdHocSlideKitId].InternalName, AdHocSlideKitId.ToString() }
                   };
        }

        public override string[] ParseFileName(string fileNameToParse)
        {
            string[] fileNameParts = base.ParseFileName(fileNameToParse);

            if (!int.TryParse(fileNameParts[1], out int tempAdhocSlideKitId))
            {
                ThrowFileNameExceptionInvalidType(fileNameToParse, SPFieldNames.AdHocSlideKitId, "Integer");
            }

            AdHocSlideKitId = tempAdhocSlideKitId;

            return fileNameParts;
        }
    }
}
