using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

using MEI.SPDocuments.Data;
using MEI.SPDocuments.TypeCodes;

namespace MEI.SPDocuments.Document
{
    [DocumentInfo(SPDocumentType.FairMarketValueToolException,
        "FairMarketValueToolException",
        "FMVTE",
        "FMVTE",
        "Fair Market Value Tool Exception")]
    public class FairMarketValueToolException
        : SPDocumentBase, ISearchSpeaker, ISearchYear
    {
        internal FairMarketValueToolException(IRepository repository, IDbUtilities dbUtilities, DocumentTypeInfo documentTypeInfo)
            : base(repository, dbUtilities, documentTypeInfo)
        { }

        public FairMarketValueToolException WithValues(int? speakerNominationId, int? speakerCounter, DocumentYear documentYear)
        {
            SpeakerCounter = speakerCounter;
            SpeakerNominationId = speakerNominationId;
            DocumentYear = documentYear;

            return this;
        }

        [SPFieldInfo(SPFieldNames.DocumentYear, "DocumentYear", SPFieldType.Text, 2)]
        public override DocumentYear DocumentYear { get; internal set; }

        [SPFieldInfo(SPFieldNames.SpeakerNominationId, "SpeakerNominationID", SPFieldType.Text, 0)]
        public int? SpeakerNominationId { get; private set; }

        [SPFieldInfo(SPFieldNames.SpeakerCounter, "SpeakerCounter", SPFieldType.Text, 1)]
        public int? SpeakerCounter { get; private set; }

        public override string FileName => MakeFileName(SpeakerNominationId, SpeakerCounter, DocumentYear.ToDisplayNameLong());

        public override bool IsValid
        {
            get
            {
                bool baseValid = base.IsValid;

                if (DocumentYear == DocumentYear.Undefined)
                {
                    return false;
                }

                if (Company == Company.AbbottNutritionCE)
                {
                    if (!SpeakerCounter.HasValue)
                    {
                        return false;
                    }
                }
                else
                {
                    if (!SpeakerNominationId.HasValue)
                    {
                        return false;
                    }
                }

                return baseValid;
            }
        }

        public override string UniqueIdentifiers => "SpeakerNominationId;SpeakerCounter";

        public override string UniqueValues => string.Format("{0};{1}", SpeakerNominationId, SpeakerCounter);

        public ISearchExpressionGroup GetSearchExpressionGroupBySpeaker(Company company, DocumentYear year, int speakerCounter)
        {
            var seg = new SearchExpressionGroup(this, SPFieldNames.SpeakerCounter, CamlComparison.Equal, speakerCounter);

            DataTable dt = Repository.GetFMVTNominationIdsBySpeakerCounter(company, year, speakerCounter);
            foreach (DataRow dr in dt.Rows)
            {
                seg.AddExpression(SPFieldNames.SpeakerNominationId, CamlComparison.Equal, DbUtilities.FromDbValue<int>(dr["ID"]));
            }

            return seg;
        }

        public ISearchExpressionGroup GetSearchExpressionGroupByYear(DocumentYear year)
        {
            if (year == DocumentYear.Undefined)
            {
                return new SearchExpressionGroup(this);
            }

            return new SearchExpressionGroup(this, SPFieldNames.DocumentYear, CamlComparison.Equal, year.ToDisplayNameLong());
        }

        public override bool ValidateFields()
        {
            if (!IsValid)
            {
                return false;
            }

            if (Company != Company.AbbottNutritionCE)
            {
                if (SpeakerNominationId != null && Repository.GetSpeakerNominationIdsBySpeakerNominationId(Company, DocumentYear, SpeakerNominationId.Value).Rows.Count <= 0)
                {
                    ThrowFileNameExceptionNoDBMatch(SPFieldNames.SpeakerNominationId, SpeakerNominationId.Value.ToString());
                }

                if (SpeakerCounter != null && Repository.GetSpeakerCountersBySpeakerCounter(Company, DocumentYear, SpeakerCounter.Value).Rows.Count <= 0)
                {
                    ThrowFileNameExceptionNoDBMatch(SPFieldNames.SpeakerCounter, SpeakerCounter.Value.ToString());
                }
            }
            else
            {
                if (SpeakerNominationId != null && Repository.GetSpeakerNominationIdsBySpeakerNominationId(Company, DocumentYear, SpeakerNominationId.Value).Rows.Count <= 0)
                {
                    ThrowFileNameExceptionNoDBMatch(SPFieldNames.SpeakerNominationId, SpeakerNominationId.Value.ToString());
                }

                if (SpeakerCounter != null && Repository.GetSpeakerCountersBySpeakerCounter(Company, DocumentYear, SpeakerCounter.Value).Rows.Count <= 0)
                {
                    ThrowFileNameExceptionNoDBMatch(SPFieldNames.SpeakerCounter, SpeakerCounter.Value.ToString());
                }
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

            SpeakerNominationId = Convert.ToInt32(objects[0]);
            if (objects[1] != null)
            {
                SpeakerCounter = Convert.ToInt32(objects[1]);
            }

            DocumentYear = objects[2].ToString().ToDocumentYear();
            Contents = (byte[])objects[3];
            FileExtension = objects[4].ToString();
            Company = (Company)objects[5];

            return IsValid;
        }

        public override bool AbstractSetup(Hashtable values)
        {
            if (values.ContainsKey(SPFields[SPFieldNames.SpeakerCounter].InternalName))
            {
                SpeakerCounter = Convert.ToInt32(values[SPFields[SPFieldNames.SpeakerCounter].InternalName]);
            }

            if (values.ContainsKey(SPFields[SPFieldNames.SpeakerNominationId].InternalName))
            {
                SpeakerNominationId =
                    Convert.ToInt32(values[SPFields[SPFieldNames.SpeakerNominationId].InternalName]);
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
                       { SPFields[SPFieldNames.SpeakerNominationId].InternalName, SpeakerNominationId.ToString() },
                       { SPFields[SPFieldNames.SpeakerCounter].InternalName, SpeakerCounter.ToString() },
                       { SPFields[SPFieldNames.DocumentYear].InternalName, DocumentYear.ToDisplayNameLong() }
                   };
        }

        public override string[] ParseFileName(string fileNameToParse)
        {
            string[] fileNameParts = base.ParseFileName(fileNameToParse);

            if (Company == Company.AbbottNutritionCE)
            {
                if (!string.IsNullOrEmpty(fileNameParts[1]))
                {
                    if (!int.TryParse(fileNameParts[1], out int tempSpeakerNominationId))
                    {
                        ThrowFileNameExceptionInvalidType(fileNameToParse, SPFieldNames.SpeakerNominationId, "Integer");
                    }

                    SpeakerNominationId = tempSpeakerNominationId;
                }
            }
            else
            {
                if (!int.TryParse(fileNameParts[1], out int tempSpeakerNominationId))
                {
                    ThrowFileNameExceptionInvalidType(fileNameToParse, SPFieldNames.SpeakerNominationId, "Integer");
                }

                SpeakerNominationId = tempSpeakerNominationId;
            }

            if (Company == Company.AbbottNutritionCE)
            {
                if (!int.TryParse(fileNameParts[2], out int tempSpeakerCounter))
                {
                    ThrowFileNameExceptionInvalidType(fileNameToParse, SPFieldNames.SpeakerCounter, "Integer");
                }

                SpeakerCounter = tempSpeakerCounter;
            }
            else
            {
                if (!string.IsNullOrEmpty(fileNameParts[2]))
                {
                    if (!int.TryParse(fileNameParts[2], out int tempSpeakerCounter))
                    {
                        ThrowFileNameExceptionInvalidType(fileNameToParse, SPFieldNames.SpeakerCounter, "Integer");
                    }

                    SpeakerCounter = tempSpeakerCounter;
                }
            }

            string tempDocumentYear = fileNameParts[3];
            DocumentYear = tempDocumentYear.ToDocumentYear();

            return fileNameParts;
        }
    }
}
