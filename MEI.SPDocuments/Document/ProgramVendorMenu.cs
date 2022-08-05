using System;
using System.Collections;
using System.Collections.Generic;

using MEI.SPDocuments.Data;
using MEI.SPDocuments.TypeCodes;

namespace MEI.SPDocuments.Document
{
    [DocumentEnabled(false)]
    [DocumentInfo(SPDocumentType.ProgramVendorMenu, "ProgramVendorMenu", "PVM", "ProgramVendorMenu", "Program Vendor Menu")]
    public class ProgramVendorMenu
        : SPDocumentBase, ISearchVendor, ISearchProgram, ISearchYear
    {
        internal ProgramVendorMenu(IRepository repository, IDbUtilities dbUtilities, DocumentTypeInfo documentTypeInfo)
            : base(repository, dbUtilities, documentTypeInfo)
        { }

        public ProgramVendorMenu WithValues(string programId, int? vendorId, MenuType menuType)
        {
            ProgramId = programId;
            MenuType = menuType;
            VendorId = vendorId;

            return this;
        }

        public override DocumentYear DocumentYear => ExtractDocumentYear(ProgramId);

        [SPFieldInfo(SPFieldNames.MenuType, "MenuType", SPFieldType.Choice, 2)]
        public MenuType MenuType { get; private set; }

        [SPFieldInfo(SPFieldNames.ProgramId, "Program_x0020_ID", SPFieldType.Text, "Program ID", 0)]
        public string ProgramId { get; private set; }

        [SPFieldInfo(SPFieldNames.VendorId, "VendorID", SPFieldType.Text, 1)]
        public int? VendorId { get; private set; }

        public override string FileName => MakeFileName(ProgramId, VendorId, MenuType.ToDisplayNameShort());

        public override bool IsValid
        {
            get
            {
                bool baseValid = base.IsValid;

                if (string.IsNullOrEmpty(ProgramId))
                {
                    return false;
                }

                if (!VendorId.HasValue)
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

        public override string UniqueIdentifiers => "ProgramId;VendorId;MenuType";

        public override string UniqueValues => string.Format("{0};{1};{2}", ProgramId, VendorId, MenuType);

        public ISearchExpressionGroup GetSearchExpressionGroupByProgram(Company company, DocumentYear year, string programId)
        {
            return new SearchExpressionGroup(this, SPFieldNames.ProgramId, CamlComparison.Equal, programId);
        }

        public ISearchExpressionGroup GetSearchExpressionGroupByVendor(Company company, DocumentYear year, int vendorId)
        {
            return new SearchExpressionGroup(this, SPFieldNames.VendorId, CamlComparison.Equal, vendorId);
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

            ProgramId = objects[0].ToString();
            VendorId = Convert.ToInt32(objects[1]);
            MenuType = objects[2].ToString().ToMenuType();
            Contents = (byte[])objects[3];
            FileExtension = objects[4].ToString();
            Company = (Company)objects[5];

            return IsValid;
        }

        public override bool AbstractSetup(Hashtable values)
        {
            if (values.ContainsKey(SPFields[SPFieldNames.ProgramId].InternalName))
            {
                ProgramId = (string)values[SPFields[SPFieldNames.ProgramId].InternalName];
            }

            if (values.ContainsKey(SPFields[SPFieldNames.VendorId].InternalName))
            {
                VendorId = Convert.ToInt32(values[SPFields[SPFieldNames.VendorId].InternalName]);
            }

            if (values.ContainsKey(SPFields[SPFieldNames.MenuType].InternalName))
            {
                MenuType = ((string)values[SPFields[SPFieldNames.MenuType].InternalName]).ToMenuType();
            }

            return true;
        }

        public override IDictionary<string, string> GetUserFieldValues()
        {
            return new Dictionary<string, string>
                   {
                       { SPFields[SPFieldNames.MenuType].InternalName, MenuType.ToDisplayNameShort() },
                       { SPFields[SPFieldNames.ProgramId].InternalName, ProgramId },
                       { SPFields[SPFieldNames.VendorId].InternalName, VendorId.ToString() }
                   };
        }

        public override string[] ParseFileName(string fileNameToParse)
        {
            string[] fileNameParts = base.ParseFileName(fileNameToParse);

            ProgramId = fileNameParts[1];

            if (!int.TryParse(fileNameParts[2], out int tempVendorId))
            {
                ThrowFileNameExceptionInvalidType(fileNameToParse, SPFieldNames.VendorId, "Integer");
            }

            VendorId = tempVendorId;
            MenuType = fileNameParts[3].ToMenuType();
            if (MenuType == MenuType.Undefined)
            {
                ThrowFileNameExceptionInvalidType(fileNameToParse, SPFieldNames.MenuType, "MenuTypeCode");
            }

            return fileNameParts;
        }
    }
}
