using System;
using System.Collections;
using System.Collections.Generic;

using MEI.SPDocuments.Data;
using MEI.SPDocuments.TypeCodes;

namespace MEI.SPDocuments.Document
{
    [DocumentEnabled(false)]
    [DocumentInfo(SPDocumentType.VendorMenu, "VendorMenu", "VM", "VendorMenu", "VendorMenu")]
    public class VendorMenu
        : SPDocumentBase
    {
        internal VendorMenu(IRepository repository, IDbUtilities dbUtilities, DocumentTypeInfo documentTypeInfo)
            : base(repository, dbUtilities, documentTypeInfo)
        { }

        public VendorMenu WithValues(int? vendorId, DateTime? startDate, DateTime? endDate, string menuTypeAcronym)
        {
            EndDate = endDate;
            MenuType = menuTypeAcronym.ToMenuType();
            StartDate = startDate;
            VendorId = vendorId;

            return this;
        }

        public override DocumentYear DocumentYear => DocumentYear.Undefined;

        [SPFieldInfo(SPFieldNames.MenuType, "MenuType", SPFieldType.Choice,3 )]
        public MenuType MenuType { get; private set; }

        [SPFieldInfo(SPFieldNames.StartDate, "StartDate", SPFieldType.DateTime, 1)]
        public DateTime? StartDate { get; private set; }

        [SPFieldInfo(SPFieldNames.VendorId, "VendorID", SPFieldType.Text, 0)]
        public int? VendorId { get; private set; }

        [SPFieldInfo(SPFieldNames.EndDate, "EndDate", SPFieldType.DateTime,2 )]
        public DateTime? EndDate { get; private set; }

        public override string FileName =>
            MakeFileName( VendorId,
                StartDate == null ? "" : StartDate.Value.ToString("MMddyyyy"),
                EndDate == null ? "" : EndDate.Value.ToString("MMddyyyy"),
                MenuType.ToDisplayNameShort());

        public override bool IsValid
        {
            get
            {
                bool baseValid = base.IsValid;

                if (!VendorId.HasValue)
                {
                    return false;
                }

                if (!StartDate.HasValue)
                {
                    return false;
                }

                if (!EndDate.HasValue)
                {
                    return false;
                }

                if (MenuType == MenuType.Undefined)
                {
                    return false;
                }

                return baseValid;
            }
        }

        public override string UniqueIdentifiers => "VendorId;StartDate;EndDate;MenuType";

        public override string UniqueValues => string.Format("{0};{1};{2};{3}", VendorId, StartDate, EndDate, MenuType);

        public override bool ValidateFields()
        {
            if (!IsValid)
            {
                return false;
            }

            if (VendorId != null && Repository.GetVendorIdsByVendorId(Company, DocumentYear, VendorId.Value).Rows.Count <= 0)
            {
                ThrowFileNameExceptionNoDBMatch(SPFieldNames.VendorId, VendorId.Value.ToString());
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

            VendorId = Convert.ToInt32(objects[0]);
            StartDate = (DateTime)objects[1];
            EndDate = (DateTime)objects[2];
            MenuType = objects[3].ToString().ToMenuType();
            Contents = (byte[])objects[4];
            FileExtension = objects[5].ToString();
            Company = (Company)objects[6];

            return IsValid;
        }

        public override bool AbstractSetup(Hashtable values)
        {
            return IsValid;
        }

        public override IDictionary<string, string> GetUserFieldValues()
        {
            return new Dictionary<string, string>
                   {
                       { SPFields[SPFieldNames.VendorId].InternalName, VendorId.ToString() },
                       { SPFields[SPFieldNames.MenuType].InternalName, MenuType.ToDisplayNameShort() }
                   };
        }

        public override string[] ParseFileName(string fileNameToParse)
        {
            string[] fileNameParts = base.ParseFileName(fileNameToParse);

            if (!int.TryParse(fileNameParts[1], out int tempVendorId))
            {
                ThrowFileNameExceptionInvalidType(fileNameToParse, SPFieldNames.VendorId, "Integer");
            }

            VendorId = tempVendorId;

            DateTime tempStartDate = default;

            if ((fileNameParts[2].Length != 8) || !DateTime.TryParse(string.Format("{0}/{1}/{2}",
                        fileNameParts[2].Substring(0, 2),
                        fileNameParts[2].Substring(2, 2),
                        fileNameParts[2].Substring(4, 4)),
                    out tempStartDate))
            {
                ThrowFileNameExceptionInvalidType(fileNameToParse, SPFieldNames.StartDate, "DateTime");
            }

            StartDate = tempStartDate;

            DateTime tempEndDate = default;

            if ((fileNameParts[3].Length != 8) || !DateTime.TryParse(string.Format("{0}/{1}/{2}",
                        fileNameParts[3].Substring(0, 2),
                        fileNameParts[3].Substring(2, 2),
                        fileNameParts[3].Substring(4, 4)),
                    out tempEndDate))
            {
                ThrowFileNameExceptionInvalidType(fileNameToParse, SPFieldNames.EndDate, "DateTime");
            }

            EndDate = tempEndDate;
            MenuType = fileNameParts[4].ToMenuType();

            if (MenuType == MenuType.Undefined)
            {
                ThrowFileNameExceptionInvalidType(fileNameToParse, SPFieldNames.MenuType, "MenuTypeCode");
            }

            return fileNameParts;
        }
    }
}
