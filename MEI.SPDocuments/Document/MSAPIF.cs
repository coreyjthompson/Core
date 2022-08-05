using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

using MEI.SPDocuments.Data;
using MEI.SPDocuments.TypeCodes;

namespace MEI.SPDocuments.Document
{
    [DocumentInfo(SPDocumentType.MSAPIF, "MSAPIF", "MSAPIF", "MSAPIF", "MovedSolvayAndroGelPIF", "MovedSolvayAndroGelPIF")]
    public class MSAPIF
        : SPDocumentBase, ISearchProgram
    {
        internal MSAPIF(IRepository repository, IDbUtilities dbUtilities, DocumentTypeInfo documentTypeInfo)
            : base(repository, dbUtilities, documentTypeInfo)
        { }

        public MSAPIF WithValues(string territory, int? pifId, DocumentYear documentYear)
        {
            PifId = pifId;
            Territory = territory;
            DocumentYear = documentYear;

            return this;
        }

        [SPFieldInfo(SPFieldNames.DocumentYear, "DocumentYear", SPFieldType.Text, 2)]
        public override DocumentYear DocumentYear { get; internal set; }

        [SPFieldInfo(SPFieldNames.PifId, "PIFID", SPFieldType.Text, 1)]
        public int? PifId { get; private set; }

        [SPFieldInfo(SPFieldNames.Territory, "Territory", SPFieldType.Text, 0)]
        public string Territory { get; private set; }

        public override string FileName => MakeFileName(Territory, PifId, DocumentYear.ToDisplayNameLong());

        public override bool IsValid
        {
            get
            {
                bool baseValid = base.IsValid;

                if (string.IsNullOrEmpty(Territory))
                {
                    return false;
                }

                if (!PifId.HasValue)
                {
                    return false;
                }

                if (DocumentYear == DocumentYear.Undefined)
                {
                    return false;
                }

                return baseValid;
            }
        }

        public override string UniqueIdentifiers => "Territory;PifId";

        public override string UniqueValues => string.Format("{0};{1}", Territory, PifId);

        public ISearchExpressionGroup GetSearchExpressionGroupByProgram(Company company, DocumentYear year, string programId)
        {
            DataTable dt = Repository.GetPifIdsByProgramId(company, year, programId);

            var seg = new SearchExpressionGroup(this)
                      {
                          BooleanLogicType = SearchBooleanLogic.Or
                      };

            foreach (DataRow dr in dt.Rows)
            {
                seg.AddExpression(SPFieldNames.PifId, CamlComparison.Equal, DbUtilities.FromDbValue<int>(dr["PifId"]));
            }

            return seg;
        }

        public override bool ValidateFields()
        {
            if (!IsValid)
            {
                return false;
            }

            if (Repository.GetTerritoriesByTerritory(Company, DocumentYear, Territory).Rows.Count <= 0)
            {
                ThrowFileNameExceptionNoDBMatch(SPFieldNames.Territory, Territory);
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

            Territory = objects[0].ToString();
            PifId = Convert.ToInt32(objects[1]);
            DocumentYear = objects[2].ToString().ToDocumentYear();
            Contents = (byte[])objects[3];
            FileExtension = objects[4].ToString();
            Company = (Company)objects[5];

            return IsValid;
        }

        public override bool AbstractSetup(Hashtable values)
        {
            if (values.ContainsKey(SPFields[SPFieldNames.PifId].InternalName))
            {
                PifId = Convert.ToInt32(values[SPFields[SPFieldNames.PifId].InternalName]);
            }

            if (values.ContainsKey(SPFields[SPFieldNames.Territory].InternalName))
            {
                Territory = (string)values[SPFields[SPFieldNames.Territory].InternalName];
            }

            if (values.ContainsKey(SPFields[SPFieldNames.DocumentYear].InternalName))
            {
                DocumentYear = ((string)values[SPFields[SPFieldNames.DocumentYear].InternalName]).ToDocumentYear();
            }

            return true;
        }

        public override IDictionary<string, string> GetUserFieldValues()
        {
            return new Dictionary<string, string>
                   {
                       { SPFields[SPFieldNames.Territory].InternalName, Territory },
                       { SPFields[SPFieldNames.PifId].InternalName, PifId.ToString() },
                       { SPFields[SPFieldNames.DocumentYear].InternalName, DocumentYear.ToDisplayNameLong() }
                   };
        }

        public override string[] ParseFileName(string fileNameToParse)
        {
            string[] fileNameParts = base.ParseFileName(fileNameToParse);

            Territory = fileNameParts[1];

            if (!int.TryParse(fileNameParts[2], out int tempPifId))
            {
                ThrowFileNameExceptionInvalidType(fileNameToParse, SPFieldNames.PifId, "Integer");
            }

            PifId = tempPifId;

            string tempDocumentYear = fileNameParts[3];
            DocumentYear = tempDocumentYear.ToDocumentYear();

            return fileNameParts;
        }
    }
}
