using System;
using System.Collections;
using System.Collections.Generic;

using MEI.SPDocuments.Data;
using MEI.SPDocuments.TypeCodes;

namespace MEI.SPDocuments.Document
{
    [DocumentInfo(SPDocumentType.EPPlacemat, "EPPlacemat", "EPPM")]
    public class EPPlacemat
        : SPDocumentBase
    {
        internal EPPlacemat(IRepository repository, IDbUtilities dbUtilities, DocumentTypeInfo documentTypeInfo)
            : base(repository, dbUtilities, documentTypeInfo)
        { }

        public EPPlacemat WithValues(int placematId, EPassStatus statusTypeCode)
        {
            PlacematId = placematId;
            Status = statusTypeCode;

            return this;
        }

        public override DocumentYear DocumentYear => DocumentYear.Undefined;

        [SPFieldInfo(SPFieldNames.PlacematId, "PlacematID", SPFieldType.Text, 0)]
        public int? PlacematId { get; private set; }

        [SPFieldInfo(SPFieldNames.StatusCode, "StatusCode", SPFieldType.Text, 1)]
        public EPassStatus Status { get; private set; }

        public override string FileName => MakeFileName(PlacematId, Status.ToDisplayNameShort());

        public override bool IsValid
        {
            get
            {
                bool baseValid = base.IsValid;

                if (!PlacematId.HasValue)
                {
                    return false;
                }

                if (Status == EPassStatus.Undefined)
                {
                    return false;
                }

                return baseValid;
            }
        }

        public override string UniqueIdentifiers => "PlacematId;Status";

        public override string UniqueValues => string.Format("{0};{1}", PlacematId, Status.ToDisplayNameLong());

        public override bool ValidateFields()
        {
            if (!IsValid)
            {
                return false;
            }

            if (!PlacematId.HasValue)
            {
                return false;
            }

            if (Status == EPassStatus.Undefined)
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

            PlacematId = Convert.ToInt32(objects[0]);
            Status = objects[1].ToString().ToEPassStatus();
            Contents = (byte[])objects[2];
            FileExtension = objects[3].ToString();
            Company = (Company)objects[4];

            return IsValid;
        }

        public override bool AbstractSetup(Hashtable values)
        {
            if (values.ContainsKey(SPFields[SPFieldNames.PlacematId].InternalName))
            {
                PlacematId = Convert.ToInt32(values[SPFields[SPFieldNames.PlacematId].InternalName]);
            }

            if (values.ContainsKey(SPFields[SPFieldNames.StatusCode].InternalName))
            {
                Status = ((string)values[SPFields[SPFieldNames.StatusCode].InternalName]).ToEPassStatus();
            }

            return true;
        }

        public override IDictionary<string, string> GetUserFieldValues()
        {
            return new Dictionary<string, string>
                   {
                       { SPFields[SPFieldNames.PlacematId].InternalName, PlacematId.ToString() },
                       { SPFields[SPFieldNames.StatusCode].InternalName, Status.ToDisplayNameShort() }
                   };
        }

        public override string[] ParseFileName(string fileNameToParse)
        {
            string[] fileNameParts = base.ParseFileName(fileNameToParse);

            if (!int.TryParse(fileNameParts[1], out int tempPlacematId))
            {
                ThrowFileNameExceptionInvalidType(fileNameToParse, SPFieldNames.PlacematId, "Integer");
            }

            PlacematId = tempPlacematId;

            Status = fileNameParts[2].ToEPassStatus();
            if (Status == EPassStatus.Undefined)
            {
                ThrowFileNameExceptionInvalidType(fileNameToParse, SPFieldNames.StatusCode, "EPassStatusCode");
            }

            return fileNameParts;
        }
    }
}
