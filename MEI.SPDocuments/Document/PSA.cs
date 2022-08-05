using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

using MEI.SPDocuments.Data;
using MEI.SPDocuments.TypeCodes;

namespace MEI.SPDocuments.Document
{
    [DocumentInfo(SPDocumentType.PSA, "PSA")]
    public class PSA
        : SPDocumentBase, ISearchYear, ISearchProgram
    {
        internal PSA(IRepository repository, IDbUtilities dbUtilities, DocumentTypeInfo documentTypeInfo)
            : base(repository, dbUtilities, documentTypeInfo)
        { }

        public PSA WithValues(string agreementId, int? speakerCounter, string speakerName)
        {
            AgreementId = agreementId;
            ProgramId = GetProgramIDForPSA(AgreementId);
            SpeakerCounter = speakerCounter;
            SpeakerName = speakerName;

            return this;
        }

        public override DocumentYear DocumentYear => ExtractDocumentYear(ProgramId);

        public override string ParsableFileName => MakeFileName(AgreementId, SpeakerCounter, SpeakerName, ProgramId);

        [SPFieldInfo(SPFieldNames.AgreementId, "Agreement_x0020_ID", SPFieldType.Text, "Agreement ID", 0)]
        public string AgreementId { get; private set; }

        [SPFieldInfo(SPFieldNames.ProgramId, "Program_x0020_ID", SPFieldType.Text, "Program ID", 3)]
        public string ProgramId { get; private set; }

        [SPFieldInfo(SPFieldNames.SpeakerCounter, "SpeakerCounter", SPFieldType.Text, 1)]
        public int? SpeakerCounter { get; private set; }

        [SPFieldInfo(SPFieldNames.SpeakerName, "SpeakerName", SPFieldType.Text, 2)]
        public string SpeakerName { get; private set; }

        public override string FileName => MakeFileName(AgreementId, SpeakerCounter, SpeakerName);

        public override bool IsValid
        {
            get
            {
                bool baseValid = base.IsValid;

                if (string.IsNullOrEmpty(AgreementId))
                {
                    return false;
                }

                if (!SpeakerCounter.HasValue)
                {
                    return false;
                }

                if (string.IsNullOrEmpty(SpeakerName))
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

        public override string UniqueIdentifiers => "AgreementId;SpeakerCounter;SpeakerName";

        public override string UniqueValues => string.Format("{0};{1};{2}", AgreementId, SpeakerCounter, SpeakerName);

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

            if (SpeakerCounter != null && Repository.GetSpeakerCountersBySpeakerCounter(Company, DocumentYear, SpeakerCounter.Value).Rows.Count <= 0)
            {
                ThrowFileNameExceptionNoDBMatch(SPFieldNames.SpeakerCounter, SpeakerCounter.Value.ToString());
            }

            //TODO: add validator for AgreementId

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

            AgreementId = objects[0].ToString();
            SpeakerCounter = Convert.ToInt32(objects[1]);
            SpeakerName = objects[2].ToString();
            Contents = (byte[])objects[3];
            FileExtension = objects[4].ToString();
            Company = (Company)objects[5];
            ProgramId = GetProgramIDForPSA(AgreementId);

            return IsValid;
        }

        public override bool AbstractSetup(Hashtable values)
        {
            if (values.ContainsKey(SPFields[SPFieldNames.ProgramId].InternalName))
            {
                ProgramId = (string)values[SPFields[SPFieldNames.ProgramId].InternalName];
            }

            if (values.ContainsKey(SPFields[SPFieldNames.AgreementId].InternalName))
            {
                AgreementId = (string)values[SPFields[SPFieldNames.AgreementId].InternalName];
            }

            if (values.ContainsKey(SPFields[SPFieldNames.SpeakerCounter].InternalName))
            {
                SpeakerCounter = Convert.ToInt32(values[SPFields[SPFieldNames.SpeakerCounter].InternalName]);
            }

            if (values.ContainsKey(SPFields[SPFieldNames.SpeakerName].InternalName))
            {
                SpeakerName = (string)values[SPFields[SPFieldNames.SpeakerName].InternalName];
            }

            return true;
        }

        public override IDictionary<string, string> GetUserFieldValues()
        {
            return new Dictionary<string, string>
                   {
                       { SPFields[SPFieldNames.AgreementId].InternalName, AgreementId },
                       { SPFields[SPFieldNames.SpeakerCounter].InternalName, SpeakerCounter.ToString() },
                       { SPFields[SPFieldNames.SpeakerName].InternalName, SpeakerName },
                       { SPFields[SPFieldNames.ProgramId].InternalName, ProgramId }
                   };
        }

        private string GetProgramIDForPSA(string agreementIdToSearch)
        {
            var conn = new SqlConnection();
            string tempP = string.Empty;

            try
            {
                if (Company == Company.Abbott)
                {
                    conn = new SqlConnection(ConfigurationManager.AppSettings["CurrentAbbott"]);
                }
                else if (Company == Company.ExactSciences)
                {
                    conn = new SqlConnection(ConfigurationManager.AppSettings["CurrentMEI"]);
                }
                else if (Company == Company.AbbottNutrition)
                {
                    conn = new SqlConnection(ConfigurationManager.AppSettings["CurrentRoss"]);
                }
                else if (Company == Company.Solvay)
                {
                    conn = new SqlConnection(ConfigurationManager.AppSettings["CurrentSolvay"]);
                }
                else if (Company == Company.LabDevelopment)
                {
                    tempP = "00008-DS02-11";
                    return tempP;
                }
                else
                {
                    throw new Exception("Company Not Set, Database To Search Unknown");
                }

                var cmd = new SqlCommand("GetProgramIDbySpkrAgmtID", conn)
                          {
                              CommandType = CommandType.StoredProcedure
                          };

                cmd.Parameters.Add(new SqlParameter("@SpkrAgmtID", SqlDbType.VarChar, 50)
                                   {
                                       Direction = ParameterDirection.Input,
                                       Value = agreementIdToSearch
                                   });
                cmd.Parameters.Add(new SqlParameter("@ProgramId", SqlDbType.VarChar, 20)
                                   {
                                       Direction = ParameterDirection.Output
                                   });

                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }

                if (conn.State == ConnectionState.Open)
                {
                    cmd.ExecuteNonQuery();
                    tempP = cmd.Parameters["@ProgramID"].Value.ToString();
                }
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }

                conn.Dispose();
            }

            return tempP;
        }

        public override string[] ParseFileName(string fileNameToParse)
        {
            string[] fileNameParts = base.ParseFileName(fileNameToParse);

            AgreementId = fileNameParts[1];

            if (!int.TryParse(fileNameParts[2], out int tempSpeakerCounter))
            {
                ThrowFileNameExceptionInvalidType(fileNameToParse, SPFieldNames.SpeakerCounter, "Integer");
            }

            SpeakerCounter = tempSpeakerCounter;
            SpeakerName = fileNameParts[3];

            if (fileNameParts[4] != null)
            {
                ProgramId = fileNameParts[4];
            }
            else
            {
                ProgramId = GetProgramIDForPSA(AgreementId);
            }

            return fileNameParts;
        }
    }
}
