using System;
using System.Collections;
using System.Collections.Generic;

using MEI.SPDocuments.Data;
using MEI.SPDocuments.TypeCodes;

namespace MEI.SPDocuments.Document
{
    [DocumentInfo(SPDocumentType.VenueException, "VenueException", "VE", "VenueException", "Venue Exception")]
    public class VenueException
        : SPDocumentBase, ISearchProgram, ISearchYear
    {
        internal VenueException(IRepository repository, IDbUtilities dbUtilities, DocumentTypeInfo documentTypeInfo)
            : base(repository, dbUtilities, documentTypeInfo)
        { }

        public VenueException WithValues(string programId, int? vendorId)
        {
            ProgramId = programId;
            VendorId = vendorId;

            return this;
        }

        public override DocumentYear DocumentYear => ExtractDocumentYear(ProgramId);

        public override string SearchFieldValue => ProgramId;

        [SPFieldInfo(SPFieldNames.VendorId, "VendorID", SPFieldType.Text, 0)]
        public int? VendorId { get; private set; }

        [SPFieldInfo(SPFieldNames.ProgramId, "ProgramId", SPFieldType.Text, 1)]
        public string ProgramId { get; private set; }

        public override string FileName => MakeFileName(ProgramId, VendorId);

        public override bool IsValid
        {
            get
            {
                bool baseValid = base.IsValid;

                if (!VendorId.HasValue)
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

        public override string UniqueIdentifiers => "VendorId;ProgramId";

        public override string UniqueValues => string.Format("{0};{1}", VendorId, ProgramId);

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
            ProgramId = objects[1].ToString();

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

            if (values.ContainsKey(SPFields[SPFieldNames.VendorId].InternalName))
            {
                VendorId = Convert.ToInt32(values[SPFields[SPFieldNames.VendorId].InternalName]);
            }

            return true;
        }

        public override IDictionary<string, string> GetUserFieldValues()
        {
            return new Dictionary<string, string>
                   {
                       { SPFields[SPFieldNames.ProgramId].InternalName, ProgramId },
                       { SPFields[SPFieldNames.VendorId].InternalName, VendorId.ToString() }
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

            ProgramId = fileNameParts[2];

            return fileNameParts;
        }
    }
}
