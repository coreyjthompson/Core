using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

using MEI.SPDocuments.Data;
using MEI.SPDocuments.TypeCodes;

namespace MEI.SPDocuments.Document
{
    [DocumentInfo(SPDocumentType.FairMarketValueTool, "FairMarketValueTool", "FMVT", "FairMarketValueTool", "Fair Market Value Tool")]
    public class FairMarketValueTool
        : SPDocumentBase, ISearchSpeaker, ISearchYear
    {
        internal FairMarketValueTool(IRepository repository, IDbUtilities dbUtilities, DocumentTypeInfo documentTypeInfo)
            : base(repository, dbUtilities, documentTypeInfo)
        { }

        public FairMarketValueTool WithValues(int? speakerNominationId, int? speakerCounter, DocumentYear documentYear)
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
            var seg = new SearchExpressionGroup(this, SPFieldNames.SpeakerCounter, CamlComparison.Equal, speakerCounter)
                      {
                          BooleanLogicType = SearchBooleanLogic.Or
                      };

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
                SpeakerNominationId = Convert.ToInt32(values[SPFields[SPFieldNames.SpeakerNominationId].InternalName]);
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

        public override WatermarkProfile GetWaterMarkProfile(string connectionString)
        {
            string sql =
                ((Company == Company.Abbott) | (Company == Company.ExactSciences))
                && DocumentYear.Compare(DocumentYear.Year2014, ">=")
                    ? "Select FMVRangesDate,IndividualFirstName +' '+ IndividualLastName as SpkrName From Speaker_nominations Where SpkrCounter = @SpkrCounter and Id = @SpeakerNominationID "
                      + "Union Select FMVRangesDate,IndividualFirstName +' '+ IndividualLastName as SpkrName From "
                      + Company.ToDisplayNameLong()
                      + Convert.ToInt32(DocumentYear.ToDisplayNameLong()) + 1
                      + ".dbo.Speaker_nominations Where SpkrCounter = @SpkrCounter and Id = @SpeakerNominationID"
                    : "Select IndividualFirstName +' '+ IndividualLastName as SpkrName From Speaker_nominations Where SpkrCounter = @SpkrCounter and Id = @SpeakerNominationID "
                      + "Union Select IndividualFirstName +' '+ IndividualLastName as SpkrName From "
                      + Company.ToDisplayNameLong()
                      + Convert.ToInt32(DocumentYear.ToDisplayNameLong()) + 1
                      + ".dbo.Speaker_nominations Where SpkrCounter = @SpkrCounter and Id = @SpeakerNominationID";

            var @params = new List<SqlParameter>
                          {
                              new SqlParameter("@SpkrCounter", SqlDbType.Int)
                              {
                                  Value = DbUtilities.ToDbValue(SpeakerCounter)
                              },
                              new SqlParameter("@SpeakerNominationID", SqlDbType.Int)
                              {
                                  Value = DbUtilities.ToDbValue(SpeakerNominationId)
                              }
                          };

            DataTable dt = DbUtilities.GetDataTable(sql, @params, connectionString, CommandType.Text);

            if (dt.Rows.Count == 0)
            {
                return new WatermarkProfile(string.Empty, 0, 0, 0, WatermarkTextDrawStyle.None, string.Empty);
            }

            return (Company == Company.Abbott)
                   && DocumentYear.Compare(DocumentYear.Year2014, ">=")
                ? new WatermarkProfile("WatermarkDocumentTiled",
                    1,
                    5,
                    30,
                    WatermarkTextDrawStyle.Outline,
                    dt.Rows[0]["SpkrName"] + " " + dt.Rows[0]["FMVRangesDate"])
                : new WatermarkProfile("WatermarkDocumentTiled",
                    1,
                    5,
                    30,
                    WatermarkTextDrawStyle.Outline,
                    dt.Rows[0]["SpkrName"].ToString());

            //Return dt.Rows(0)("Spkrfn").ToString & " " & dt.Rows(0)("Spkrln").ToString
            //Return String.Empty
        }
    }
}
