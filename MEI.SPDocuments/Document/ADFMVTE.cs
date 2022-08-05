using System;
using System.Collections;
using System.Collections.Generic;

using MEI.SPDocuments.Data;
using MEI.SPDocuments.TypeCodes;

namespace MEI.SPDocuments.Document
{
    [DocumentInfo(SPDocumentType.ADFMVTE, "ADFMVTE")]
    public class ADFMVTE
        : SPDocumentBase, ISearchTrackNumber
    {
        internal ADFMVTE(IRepository repository, IDbUtilities dbUtilities, DocumentTypeInfo documentTypeInfo)
            : base(repository, dbUtilities, documentTypeInfo)
        { }

        public ADFMVTE WithValues(int? trackNumber, string hcpFirstName, string hcpLastName)
        {
            TrackNumber = trackNumber;
            HcpFirstName = hcpFirstName;
            HcpLastName = hcpLastName;

            return this;
        }

        [SPFieldInfo(SPFieldNames.TrackNumber, "TrackNum", SPFieldType.Text, 0)]
        public int? TrackNumber { get; private set; }

        [SPFieldInfo(SPFieldNames.HcpFirstName, "HCPFName", SPFieldType.Text, 1)]
        public string HcpFirstName { get; private set; }

        [SPFieldInfo(SPFieldNames.HcpLastName, "HCPLName", SPFieldType.Text, 2)]
        public string HcpLastName { get; private set; }

        public override string FileName => MakeFileName(TrackNumber, HcpFirstName, HcpLastName);

        public override bool IsValid
        {
            get
            {
                bool baseValid = base.IsValid;

                if (!TrackNumber.HasValue)
                {
                    return false;
                }

                if (string.IsNullOrEmpty(HcpFirstName))
                {
                    return false;
                }

                if (string.IsNullOrEmpty(HcpLastName))
                {
                    return false;
                }

                return baseValid;
            }
        }

        public override IList<string> AllowedFileTypes =>
            new List<string>
            {
                "pdf",
                "ppt",
                "pptx",
                "doc",
                "docx",
                "txt"
            };

        public override string UniqueIdentifiers => "TrackNumber;HcpFirstName;HcpLastName";

        public override string UniqueValues => string.Format("{0};{1};{2}", TrackNumber, HcpFirstName, HcpLastName);

        public ISearchExpressionGroup GetSearchExpressionGroupByTrackNumber(Company company, DocumentYear year, int trackNumber)
        {
            return new SearchExpressionGroup(this, SPFieldNames.TrackNumber, CamlComparison.Equal, trackNumber);
        }

        public ISearchExpressionGroup GetSearchExpressionGroupByHcpName(Company company,
                                                                        DocumentYear year,
                                                                        string hcpFirstName,
                                                                        string hcpLastName)
        {
            var seg = new SearchExpressionGroup(this)
                      {
                          BooleanLogicType = SearchBooleanLogic.Or
                      };

            if (!string.IsNullOrEmpty(hcpFirstName))
            {
                seg.AddExpression(SPFieldNames.HcpFirstName, CamlComparison.Equal, hcpFirstName);
            }

            if (!string.IsNullOrEmpty(hcpLastName))
            {
                seg.AddExpression(SPFieldNames.HcpLastName, CamlComparison.Equal, hcpLastName);
            }

            return seg;
        }

        public override bool ValidateFields()
        {
            if (!IsValid)
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

            TrackNumber = Convert.ToInt32(objects[0]);
            HcpFirstName = objects[1].ToString();
            HcpLastName = objects[2].ToString();
            Contents = (byte[])objects[3];
            FileExtension = objects[4].ToString();
            Company = (Company)objects[5];

            return IsValid;
        }

        public override bool AbstractSetup(Hashtable values)
        {
            if (values.ContainsKey(SPFields[SPFieldNames.TrackNumber].InternalName))
            {
                TrackNumber = Convert.ToInt32(values[SPFields[SPFieldNames.TrackNumber].InternalName]);
            }

            if (values.ContainsKey(SPFields[SPFieldNames.HcpFirstName].InternalName))
            {
                HcpFirstName = (string)values[SPFields[SPFieldNames.HcpFirstName].InternalName];
            }

            if (values.ContainsKey(SPFields[SPFieldNames.HcpLastName].InternalName))
            {
                HcpLastName = (string)values[SPFields[SPFieldNames.HcpLastName].InternalName];
            }

            return true;
        }

        public override IDictionary<string, string> GetUserFieldValues()
        {
            return new Dictionary<string, string>
                   {
                       { SPFields[SPFieldNames.TrackNumber].InternalName, TrackNumber.ToString() },
                       { SPFields[SPFieldNames.FileName].InternalName, HcpFirstName },
                       { SPFields[SPFieldNames.HcpLastName].InternalName, HcpLastName }
                   };
        }

        public override string[] ParseFileName(string fileNameToParse)
        {
            string[] fileNameParts = base.ParseFileName(fileNameToParse);

            if (!int.TryParse(fileNameParts[1], out int tempTrackNum))
            {
                ThrowFileNameExceptionInvalidType(fileNameToParse, SPFieldNames.TrackNumber, "Integer");
            }

            TrackNumber = tempTrackNum;

            if (string.IsNullOrEmpty(fileNameParts[2]))
            {
                ThrowFileNameExceptionInvalidType(fileNameToParse, SPFieldNames.HcpFirstName, "String");
            }

            HcpFirstName = fileNameParts[2];

            if (string.IsNullOrEmpty(fileNameParts[3]))
            {
                ThrowFileNameExceptionInvalidType(fileNameToParse, SPFieldNames.HcpLastName, "String");
            }

            HcpLastName = fileNameParts[3];

            return fileNameParts;
        }
    }
}
