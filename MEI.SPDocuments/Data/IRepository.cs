using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

using MEI.SPDocuments.Security;
using MEI.SPDocuments.TypeCodes;

using Microsoft.Extensions.Options;

namespace MEI.SPDocuments.Data
{
    public interface IRepository
    {
        /// <summary>
        ///     Gets the latest index for a document type to append to file names.
        /// </summary>
        /// <param name="documentType"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        int GetDocXIndex(string documentType);

        /// <summary>
        ///     Gets program ids by the specified <paramref name="programId" />.
        /// </summary>
        /// <param name="company">The company of the connection string.</param>
        /// <param name="year">The year of the connection string.</param>
        /// <param name="programId">The program id to search on.</param>
        /// <returns>A <see cref="DataTable" /> containing the program ids.</returns>
        DataTable GetProgramIdsByProgramId(Company company, DocumentYear year, string programId);

        /// <summary>
        ///     Gets Programs in a given program's series
        /// </summary>
        /// <param name="company"></param>
        /// <param name="year"></param>
        /// <param name="programId"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        DataTable GetProgsInSeriesByProgramId(Company company, DocumentYear year, string programId);

        /// <summary>
        ///     Gets participant counters by the specified <paramref name="participantCounter" />.
        /// </summary>
        /// <param name="company">The company of the connection string.</param>
        /// <param name="year">The year of the connection string.</param>
        /// <param name="participantCounter">The participant counter to search on.</param>
        /// <returns>A <see cref="DataTable" /> containing the participant counters.</returns>
        DataTable GetParticipantCounters(Company company, DocumentYear year, int? participantCounter);

        /// <summary>
        ///     Matches an approval ID to a programID
        /// </summary>
        /// <param name="company"></param>
        /// <param name="year"></param>
        /// <param name="programId"></param>
        /// <param name="approvalId"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        DataTable GetApprovalIdByProgramId(Company company, DocumentYear year, string programId, string approvalId);

        /// <summary>
        ///     Gets speaker counters by the specified <paramref name="speakerCounter" />.
        /// </summary>
        /// <param name="company">The company of the connection string.</param>
        /// <param name="year">The year of the connection string.</param>
        /// <param name="speakerCounter">The speaker counter to search on.</param>
        /// <returns>A <see cref="DataTable" /> containing the speaker counters.</returns>
        DataTable GetSpeakerCountersBySpeakerCounter(Company company, DocumentYear year, int speakerCounter);

        /// <summary>
        ///     Gets expense counters by the specified <paramref name="expenseCounter" />.
        /// </summary>
        /// <param name="company">The company of the connection string.</param>
        /// <param name="year">The year of the connection string.</param>
        /// <param name="expenseCounter">The expense counter to search on.</param>
        /// <returns>A <see cref="DataTable" /> containing the expense counters.</returns>
        DataTable GetExpenseCountersByExpenseCounter(Company company, DocumentYear year, int expenseCounter);

        /// <summary>
        ///     Gets vendor ids by the specified <paramref name="vendorId" />.
        /// </summary>
        /// <param name="company">The company of the connection string.</param>
        /// <param name="year">The year of the connection string.</param>
        /// <param name="vendorId">The vendorId to search on.</param>
        /// <returns>A <see cref="DataTable" /> containing the vendor ids.</returns>
        DataTable GetVendorIdsByVendorId(Company company, DocumentYear year, int vendorId);

        /// <summary>
        ///     Gets invite ids by the specified <paramref name="inviteId" />.
        /// </summary>
        /// <param name="company">The company of the connection string.</param>
        /// <param name="year">The year of the connection string.</param>
        /// <param name="inviteId">The invite id to search on.</param>
        /// <returns>A <see cref="DataTable" /> containing the invite ids.</returns>
        DataTable GetInviteIdsByInviteId(Company company, DocumentYear year, int inviteId);

        /// <summary>
        ///     Gets participant counters by the specified <paramref name="programId" />.
        /// </summary>
        /// <param name="company">The company of the connection string.</param>
        /// <param name="year">The year of the connection string.</param>
        /// <param name="programId">The program id to search on.</param>
        /// <returns>A <see cref="DataTable" /> containing the participant counters.</returns>
        DataTable GetParticipantCounterByProgramId(Company company, DocumentYear year, string programId);

        /// <summary>
        ///     Gets territories by the specified <paramref name="territory" />.
        /// </summary>
        /// <param name="company">The company of the connection string.</param>
        /// <param name="year">The year of the connection string.</param>
        /// <param name="territory">The territory to search on.</param>
        /// <returns>A <see cref="DataTable" /> containing the territories.</returns>
        DataTable GetTerritoriesByTerritory(Company company, DocumentYear year, string territory);

        /// <summary>
        ///     Gets speaker nomination ids by the specified <paramref name="speakerNominationId" />.
        /// </summary>
        /// <param name="company">The company of the connection string.</param>
        /// <param name="year">The year of the connection string.</param>
        /// <param name="speakerNominationId">The speaker nomination id to search on.</param>
        /// <returns>A <see cref="DataTable" /> containing the speaker nomination ids.</returns>
        DataTable GetSpeakerNominationIdsBySpeakerNominationId(Company company, DocumentYear year, int speakerNominationId);

        /// <summary>
        ///     Gets document search document type ids by the specified <paramref name="documentSearchDocumentTypeId" />.
        /// </summary>
        /// <param name="company">The company of the connection string.</param>
        /// <param name="year">The year of the connection string.</param>
        /// <param name="documentSearchDocumentTypeId">The document search document type id to search on.</param>
        /// <returns>A <see cref="DataTable" /> containing the document search document type ids.</returns>
        DataTable GetDocumentSearchDocumentTypeIdsByDocumentSearchDocumentTypeId(Company company,
                                                                                 DocumentYear year,
                                                                                 long documentSearchDocumentTypeId);

        /// <summary>
        ///     Gets save the date ids by the specified <paramref name="saveTheDateId" />.
        /// </summary>
        /// <param name="company">The company of the connection string.</param>
        /// <param name="year">The year of the connection string.</param>
        /// <param name="saveTheDateId">The save the date id to search on.</param>
        /// <returns>A <see cref="DataTable" /> containing the save the date ids.</returns>
        DataTable GetSaveTheDateIdsBySaveTheDateId(Company company, DocumentYear year, int saveTheDateId);

        /// <summary>
        ///     Gets adhoc slidekit ids by the specified <paramref name="speakerCounter" />.
        /// </summary>
        /// <param name="company">The company of the connection string.</param>
        /// <param name="year">The year of the connection string.</param>
        /// <param name="speakerCounter">The speaker counter to search on.</param>
        /// <returns>A <see cref="DataTable" /> containing the adhoc slidekit ids.</returns>
        DataTable GetAdhocSlidekitIdsBySpeakerCounter(Company company, DocumentYear year, int speakerCounter);

        /// <summary>
        ///     Gets fmvt nomination ids by the specified <paramref name="speakerCounter" />.
        /// </summary>
        /// <param name="company">The company of the connection string.</param>
        /// <param name="year">The year of the connection string.</param>
        /// <param name="speakerCounter">The speaker counter to search on.</param>
        /// <returns>A <see cref="DataTable" /> containing the fmvt nomination ids.</returns>
        DataTable GetFMVTNominationIdsBySpeakerCounter(Company company, DocumentYear year, int speakerCounter);

        /// <summary>
        ///     Gets nomination ids by the specified <paramref name="speakerCounter" />.
        /// </summary>
        /// <param name="company">The company of the connection string.</param>
        /// <param name="year">The year of the connection string.</param>
        /// <param name="speakerCounter">The speaker counter to search on.</param>
        /// <returns>A <see cref="DataTable" /> containing the nomination ids.</returns>
        DataTable GetSNNominationIdsBySpeakerCounter(Company company, DocumentYear year, int speakerCounter);

        /// <summary>
        ///     Gets adhoc slidekit ids by the specified <paramref name="programId" />.
        /// </summary>
        /// <param name="company">The company of the connection string.</param>
        /// <param name="year">The year of the connection string.</param>
        /// <param name="programId">The program id to search on.</param>
        /// <returns>A <see cref="DataTable" /> containing the adhoc slidekit ids.</returns>
        DataTable GetAdhocSlidekitIdsByProgramId(Company company, DocumentYear year, string programId);

        /// <summary>
        ///     Gets slidekit ids by the specified <paramref name="speakerCounter" />.
        /// </summary>
        /// <param name="company">The company of the connection string.</param>
        /// <param name="year">The year of the connection string.</param>
        /// <param name="speakerCounter">The speaker counter to search on.</param>
        /// <returns>A <see cref="DataTable" /> containing the slidekit ids.</returns>
        DataTable GetSlideKitIdBySpeakerCounter(Company company, DocumentYear year, int speakerCounter);

        /// <summary>
        ///     Gets pif ids by the specified <paramref name="programId" />.
        /// </summary>
        /// <param name="company">The company of the connection string.</param>
        /// <param name="year">The year of the connection string.</param>
        /// <param name="programId">The program id to search on.</param>
        /// <returns>A <see cref="DataTable" /> containing the pif ids.</returns>
        DataTable GetPifIdsByProgramId(Company company, DocumentYear year, string programId);

        /// <summary>
        ///     Gets expense counters by the specified <paramref name="vendorId" />.
        /// </summary>
        /// <param name="company">The company of the connection string.</param>
        /// <param name="year">The year of the connection string.</param>
        /// <param name="vendorId">The vendor id to search on.</param>
        /// <returns>A <see cref="DataTable" /> containing the expense counters.</returns>
        DataTable GetExpenseCountersByVendorId(Company company, DocumentYear year, int vendorId);

        /// <summary>
        ///     Creates and inserts a document log into the database.
        /// </summary>
        /// <param name="documentAcronym">The document acronym.</param>
        /// <param name="fileName">The fileName of the document.</param>
        /// <param name="modifiedDate">The modified date.</param>
        /// <param name="status">The resulting status of the action done to the document.</param>
        /// <param name="actionType">Type of the action done on the document.</param>
        /// <param name="isError">
        ///     if set to <c>true</c> then the log entry represents an error, else it does not represent an
        ///     error.
        /// </param>
        /// <param name="message">The message body of the log entry.</param>
        /// <param name="company">The company the document belongs to.</param>
        /// <param name="userName">Name of the user.</param>
        /// <returns>An <see cref="Int32" /> equal to the number of rows affected.</returns>
        int InsertDocumentLog(string documentAcronym,
                              string fileName,
                              DateTime modifiedDate,
                              SPActionStatus status,
                              SPDocumentPrivileges actionType,
                              bool isError,
                              string message,
                              Company company,
                              string userName);

        /// <summary>
        ///     Inserts a barcode entry into the database.
        /// </summary>
        /// <param name="documentAcronym">The document acronym.</param>
        /// <param name="fileName">Name of the file whose barcode is being inserted.</param>
        /// <param name="company">The company used to determine the database name.</param>
        /// <returns>The barcodeId of the document.</returns>
        int InsertBarcodeEntry(string documentAcronym, string fileName, Company company);

        /// <summary>
        ///     Gets the user's document privileges from the database.
        /// </summary>
        /// <param name="documentType">
        ///     Type of the document to get the privileges for.  f none, then the privileges for
        ///     all document types will be returned.
        /// </param>
        /// <param name="domainGroupsForUser">The domain groups for user.</param>
        /// <returns>A DocumentPrivilegesCollection containing the privileges of the specified domain groups.</returns>
        IList<DocumentAccessInfo> GetDocumentAccessInfos(SPDocumentType documentType, IList<string> domainGroupsForUser);

        int CreateTicklersOnSignInSheetUpload(string programId, Company company, DocumentYear year);

        int CreateTicklersOnProgramAttestationUpload(string programId, Company company, DocumentYear year);

        int InsertIntoReceiptsOnReceiptUpload(string programId, int? expenseCounter, Company company, DocumentYear year);

        int UpdateSpeakerBuiltSlideKitOnUpload(string programId, int speakerCounter, DocumentYear year);

        int GetSequenceNumberForAttendeeProgramEvaluation(Company company, DocumentYear year, string programId, int pageCount);

        DataTable GetSecondAddressAmendmentProofById(Company company, DocumentYear year, int secondAddressAmendmentId);

        void SendInviteApprovalEmail(Company company, DocumentYear year, string programId);

        string GetInvoiceNumberForTravelDocId(int travelDocId);

        string GetRecordLocatorForTravelDocId(int travelDocId);

        string GetTicketNumberForTravelDocId(int travelDocId);

        string[] GetApprovedDocumentDeletes();
    }

    /// <summary>
    /// A helper class used for common database data retrievals and inserts.
    /// </summary>
    internal class Repository
        : IRepository
    {
        private readonly IDbUtilities _dbUtilities;
        private readonly SPDocumentsOptions _options;
        private readonly ISPDocumentsOptionsAggregator _optionsAggregator;
        private readonly IDocumentInfoAggregator _documentInfoAggregator;

        public Repository(IDbUtilities dbUtilities,
                          IOptions<SPDocumentsOptions> options,
                          ISPDocumentsOptionsAggregator optionsAggregator,
                          IDocumentInfoAggregator documentInfoAggregator)
        {
            _dbUtilities = dbUtilities;
            _options = options.Value;
            _optionsAggregator = optionsAggregator;
            _documentInfoAggregator = documentInfoAggregator;
        }

        /// <summary>
        /// Gets the latest index for a document type to append to file names.
        /// </summary>
        /// <param name="documentType"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public int GetDocXIndex(string documentType)
        {
            const string sql = "usp_Increment_And_Get_New_xIndex";
            var parameter = new SqlParameter("@DocType", SqlDbType.VarChar, 50)
            {
                Value = _dbUtilities.ToDbValue(documentType)
            };
            string connectionString = _options.ConnectionStrings.Event;

            return Convert.ToInt32(_dbUtilities
                .GetDataTable(sql, parameter, connectionString, CommandType.StoredProcedure).Rows[0]["xIndex"]);
        }

        public DataTable GetApprovalIdByProgramId(Company company, DocumentYear year, string programId, string approvalId)
        {
            const string sql = "Select * From Program Where programID = @ProgramID and ThirdPartyProjectNumber = @ApprovalNumber";

            var parameters = new List<SqlParameter>
                          {
                              new SqlParameter("@ProgramID", SqlDbType.VarChar, 50)
                              {
                                  Value = _dbUtilities.ToDbValue(programId)
                              },
                              new SqlParameter("@ApprovalNumber", SqlDbType.VarChar, 50)
                              {
                                  Value = _dbUtilities.ToDbValue(approvalId)
                              }
                          };

            return _dbUtilities.GetDataTable(sql,
                parameters,
                _optionsAggregator.GetCompanyConnectionString(company, year),
                CommandType.Text);
        }

        /// <summary>
        /// Gets program ids by the specified <paramref name="programId"/>.
        /// </summary>
        /// <param name="company">The company of the connection string.</param>
        /// <param name="year">The year of the connection string.</param>
        /// <param name="programId">The program id to search on.</param>
        /// <returns>A <see cref="DataTable"/> containing the program ids.</returns>
        public DataTable GetProgramIdsByProgramId(Company company, DocumentYear year, string programId)
        {
            string sql = company == Company.AbbottNutritionCE
                ? "SELECT ProgramID FROM ProgramSeries WHERE ProgramID = @ProgramID;"
                : "SELECT ProgramID FROM Program WHERE ProgramID = @ProgramID;";

            var parameter = new SqlParameter("ProgramID", SqlDbType.VarChar, 20)
            {
                Value = _dbUtilities.ToDbValue(programId)
            };

            return _dbUtilities.GetDataTable(sql,
                parameter,
                _optionsAggregator.GetCompanyConnectionString(company, year),
                CommandType.Text);
        }

        /// <summary>
        /// Gets Programs in a given program's series
        /// </summary>
        /// <param name="company"></param>
        /// <param name="year"></param>
        /// <param name="programId"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public DataTable GetProgsInSeriesByProgramId(Company company, DocumentYear year, string programId)
        {
            string sql = company == Company.AbbVieTrain
                ? "Select * From Program"
                : "Select * From ProgsInSeries Where SeriesID = (Select SeriesID From ProgsInSeries Where ProgramID = @ProgramID) And ProgramID <> @ProgramID";
            var param = new SqlParameter("@ProgramID", SqlDbType.VarChar, 50)
            {
                Value = _dbUtilities.ToDbValue(programId)
            };

            DataTable dt = _dbUtilities.GetDataTable(sql,
                param,
                _optionsAggregator.GetCompanyConnectionString(company, year),
                CommandType.Text);

            return dt;
        }

        /// <summary>
        /// Gets participant counters by the specified <paramref name="participantCounter"/>.
        /// </summary>
        /// <param name="company">The company of the connection string.</param>
        /// <param name="year">The year of the connection string.</param>
        /// <param name="participantCounter">The participant counter to search on.</param>
        /// <returns>A <see cref="DataTable"/> containing the participant counters.</returns>
        public DataTable GetParticipantCounters(Company company, DocumentYear year, int? participantCounter)
        {
            const string sql = "Select * From Participant Where ParticipantCounter = @ParticipantCounter";
            var parameter = new SqlParameter("ParticipantCounter", SqlDbType.Int)
            {
                Value = _dbUtilities.ToDbValue(participantCounter)
            };

            return _dbUtilities.GetDataTable(sql,
                parameter,
                _optionsAggregator.GetCompanyConnectionString(company, year),
                CommandType.Text);
        }

        /// <summary>
        /// Gets speaker counters by the specified <paramref name="speakerCounter"/>.
        /// </summary>
        /// <param name="company">The company of the connection string.</param>
        /// <param name="year">The year of the connection string.</param>
        /// <param name="speakerCounter">The speaker counter to search on.</param>
        /// <returns>A <see cref="DataTable"/> containing the speaker counters.</returns>
        public DataTable GetSpeakerCountersBySpeakerCounter(Company company, DocumentYear year, int speakerCounter)
        {
            string sql;

            if (company == Company.Solvay
                || (company == Company.Abbott && (year == DocumentYear.Year2009 || year == DocumentYear.Year2010)))
            {
                sql = "SELECT SpkrCounter FROM Abbott2005.dbo.Speaker WHERE SpkrCounter = @SpkrCounter;";
            }
            else if (company == Company.LipoScience && year == DocumentYear.Year2010)
            {
                sql = "SELECT SpkrCounter FROM LipoScienceSpeaker WHERE SpkrCounter = @SpkrCounter;";
            }
            else if (company == Company.Abbott && year.Compare(DocumentYear.Year2014, "<=")
                                                   && year.Compare(DocumentYear.Year2012, ">="))
            {
                sql =
                    "SELECT SpkrCounter FROM Speaker WHERE SpkrCounter = @SpkrCounter UNION SELECT SpkrCounter FROM AtherotechSpeaker WHERE SpkrCounter = @SpkrCounter;";

                //AteroTechSpeaker database gone after 2015
            }
            else if ((company == Company.Abbott | company == Company.ExactSciences)
                     && year.Compare(DocumentYear.Year2015, ">="))
            {
                sql = "SELECT SpkrCounter FROM Speaker WHERE SpkrCounter = @SpkrCounter";
            }
            else if (company == Company.AbbVieTrain)
            {
                sql = "SELECT SpkrCounter FROM tblParticipants Where SpkrCounter = @SpkrCounter";
            }
            else
            {
                sql = "SELECT SpkrCounter FROM Speaker WHERE SpkrCounter = @SpkrCounter;";
            }

            var parameter = new SqlParameter("SpkrCounter", SqlDbType.Int)
            {
                Value = _dbUtilities.ToDbValue(speakerCounter)
            };

            return _dbUtilities.GetDataTable(sql,
                parameter,
                _optionsAggregator.GetCompanyConnectionString(company, year),
                CommandType.Text);
        }

        /// <summary>
        /// Gets expense counters by the specified <paramref name="expenseCounter"/>.
        /// </summary>
        /// <param name="company">The company of the connection string.</param>
        /// <param name="year">The year of the connection string.</param>
        /// <param name="expenseCounter">The expense counter to search on.</param>
        /// <returns>A <see cref="DataTable"/> containing the expense counters.</returns>
        public DataTable GetExpenseCountersByExpenseCounter(Company company, DocumentYear year, int expenseCounter)
        {
            const string sql = "SELECT ExpenseCounter FROM Expenses WHERE ExpenseCounter = @ExpenseCounter;";
            var parameter = new SqlParameter("ExpenseCounter", SqlDbType.Int)
            {
                Value = _dbUtilities.ToDbValue(expenseCounter)
            };

            return _dbUtilities.GetDataTable(sql,
                parameter,
                _optionsAggregator.GetCompanyConnectionString(company, year),
                CommandType.Text);
        }

        /// <summary>
        /// Gets vendor ids by the specified <paramref name="vendorId"/>.
        /// </summary>
        /// <param name="company">The company of the connection string.</param>
        /// <param name="year">The year of the connection string.</param>
        /// <param name="vendorId">The vendorId to search on.</param>
        /// <returns>A <see cref="DataTable"/> containing the vendor ids.</returns>
        public DataTable GetVendorIdsByVendorId(Company company, DocumentYear year, int vendorId)
        {
            string sql;

            if (company == Company.Kowa)
            {
                sql = "SELECT VendorID FROM KowaVendors.dbo.Vendors WHERE VendorID = @VendorID;";
            }
            else if (company == Company.AbbottNutritionCE)
            {
                sql = "SELECT VendorID FROM ANHIVendors.dbo.Vendors WHERE VendorID = @VendorID;";
            }
            else if (company == Company.Quest)
            {
                sql = "Select VendorID From BerkeleyVendors.dbo.Vendors Where VendorID = @VendorID;";
            }
            else if (company == Company.Alkermes)
            {
                sql = "SELECT VendorID From AlkermesVendors.dbo.Vendors WHERE VendorID = @VendorID;";
            }
            else if (company == Company.ExactSciences)
            {
                sql = "SELECT VendorID From MEI2019.dbo.Vendors WHERE VendorID = @VendorID;";
            }
            else
            {
                sql = "SELECT VendorID FROM Vendors2007.dbo.Vendors WHERE VendorID = @VendorID;";
            }

            var parameter = new SqlParameter("VendorID", SqlDbType.Int)
            {
                Value = _dbUtilities.ToDbValue(vendorId)
            };

            return _dbUtilities.GetDataTable(sql,
                parameter,
                _optionsAggregator.GetCompanyConnectionString(company, year),
                CommandType.Text);
        }

        /// <summary>
        /// Gets invite ids by the specified <paramref name="inviteId"/>.
        /// </summary>
        /// <param name="company">The company of the connection string.</param>
        /// <param name="year">The year of the connection string.</param>
        /// <param name="inviteId">The invite id to search on.</param>
        /// <returns>A <see cref="DataTable"/> containing the invite ids.</returns>
        public DataTable GetInviteIdsByInviteId(Company company, DocumentYear year, int inviteId)
        {
            const string sql = "SELECT InviteID FROM Invite_lines where InviteID = @InviteID;";
            var parameter = new SqlParameter("InviteID", SqlDbType.Int)
            {
                Value = _dbUtilities.ToDbValue(inviteId)
            };

            return _dbUtilities.GetDataTable(sql,
                parameter,
                _optionsAggregator.GetCompanyConnectionString(company, year),
                CommandType.Text);
        }

        /// <summary>
        /// Gets participant counters by the specified <paramref name="programId"/>.
        /// </summary>
        /// <param name="company">The company of the connection string.</param>
        /// <param name="year">The year of the connection string.</param>
        /// <param name="programId">The program id to search on.</param>
        /// <returns>A <see cref="DataTable"/> containing the participant counters.</returns>
        public DataTable GetParticipantCounterByProgramId(Company company, DocumentYear year, string programId)
        {
            const string sql = "usp_GetParticipantCounterByProgramID";
            var parameter = new SqlParameter("ProgramID", SqlDbType.VarChar, 10)
            {
                Value = _dbUtilities.ToDbValue(programId)
            };

            return _dbUtilities.GetDataTable(sql,
                parameter,
                _optionsAggregator.GetCompanyConnectionString(company, year),
                CommandType.StoredProcedure);
        }

        /// <summary>
        /// Gets territories by the specified <paramref name="territory"/>.
        /// </summary>
        /// <param name="company">The company of the connection string.</param>
        /// <param name="year">The year of the connection string.</param>
        /// <param name="territory">The territory to search on.</param>
        /// <returns>A <see cref="DataTable"/> containing the territories.</returns>
        public DataTable GetTerritoriesByTerritory(Company company, DocumentYear year, string territory)
        {
            const string sql = "SELECT Terr FROM Territory_Reps WHERE Terr = @Terr;";
            var parameter = new SqlParameter("Terr", SqlDbType.VarChar, 50)
            {
                Value = _dbUtilities.ToDbValue(territory)
            };

            return _dbUtilities.GetDataTable(sql,
                parameter,
                _optionsAggregator.GetCompanyConnectionString(company, year),
                CommandType.Text);
        }

        /// <summary>
        /// Gets speaker nomination ids by the specified <paramref name="speakerNominationId"/>.
        /// </summary>
        /// <param name="company">The company of the connection string.</param>
        /// <param name="year">The year of the connection string.</param>
        /// <param name="speakerNominationId">The speaker nomination id to search on.</param>
        /// <returns>A <see cref="DataTable"/> containing the speaker nomination ids.</returns>
        public DataTable GetSpeakerNominationIdsBySpeakerNominationId(Company company, DocumentYear year, int speakerNominationId)
        {
            string previousYear = company.ToDisplayNameLong()
                                  + (Convert.ToInt32(year.ToDisplayNameLong()) - 1);
            string nextYear = company.ToDisplayNameLong()
                              + (Convert.ToInt32(year.ToDisplayNameLong()) + 1);

            string sql = string.Format(
                "SELECT ID FROM Speaker_Nominations WHERE ID = @ID UNION Select ID From {0}.dbo.Speaker_Nominations Where ID = @ID UNION Select ID From {1}.dbo.Speaker_Nominations Where ID = @ID",
                previousYear,
                nextYear);
            var parameter = new SqlParameter("ID", SqlDbType.Int)
            {
                Value = _dbUtilities.ToDbValue(speakerNominationId)
            };

            try
            {
                return _dbUtilities.GetDataTable(sql,
                    parameter,
                    _optionsAggregator.GetCompanyConnectionString(company, year),
                    CommandType.Text);
            }
            catch(Exception)
            {
                try
                {
                    sql = string.Format(
                        "SELECT ID FROM Speaker_Nominations WHERE ID = @ID UNION Select ID From {0}.dbo.Speaker_Nominations Where ID = @ID",
                        previousYear);
                    var param = new SqlParameter("ID", SqlDbType.Int)
                    {
                        Value = _dbUtilities.ToDbValue(speakerNominationId)
                    };

                    return _dbUtilities.GetDataTable(sql,
                        param,
                        _optionsAggregator.GetCompanyConnectionString(company, year),
                        CommandType.Text);
                }
                catch (Exception)
                {
                    sql = "SELECT ID FROM Speaker_Nominations WHERE ID = @ID";
                    var param = new SqlParameter("ID", SqlDbType.Int)
                    {
                        Value = _dbUtilities.ToDbValue(speakerNominationId)
                    };

                    return _dbUtilities.GetDataTable(sql,
                        param,
                        _optionsAggregator.GetCompanyConnectionString(company, year),
                        CommandType.Text);
                }
            }
        }

        /// <summary>
        /// Gets document search document type ids by the specified <paramref name="documentSearchDocumentTypeId"/>.
        /// </summary>
        /// <param name="company">The company of the connection string.</param>
        /// <param name="year">The year of the connection string.</param>
        /// <param name="documentSearchDocumentTypeId">The document search document type id to search on.</param>
        /// <returns>A <see cref="DataTable"/> containing the document search document type ids.</returns>
        public DataTable GetDocumentSearchDocumentTypeIdsByDocumentSearchDocumentTypeId(
            Company company,
            DocumentYear year,
            long documentSearchDocumentTypeId)
        {
            const string sql =
                "SELECT DocumentSearchDocumentTypeId FROM DocumentSearchDocumentType WHERE DocumentSearchDocumentTypeId = @DocumentSearchDocumentTypeId;";
            var parameter = new SqlParameter("DocumentSearchDocumentTypeId", SqlDbType.BigInt)
            {
                Value = _dbUtilities.ToDbValue(documentSearchDocumentTypeId)
            };

            return _dbUtilities.GetDataTable(sql,
                parameter,
                _optionsAggregator.GetCompanyConnectionString(company, year),
                CommandType.Text);
        }

        /// <summary>
        /// Gets save the date ids by the specified <paramref name="saveTheDateId"/>.
        /// </summary>
        /// <param name="company">The company of the connection string.</param>
        /// <param name="year">The year of the connection string.</param>
        /// <param name="saveTheDateId">The save the date id to search on.</param>
        /// <returns>A <see cref="DataTable"/> containing the save the date ids.</returns>
        public DataTable GetSaveTheDateIdsBySaveTheDateId(Company company, DocumentYear year, int saveTheDateId)
        {
            const string sql = "SELECT STDID FROM Save_The_Date WHERE STDID = @STDID;";
            var parameter = new SqlParameter("STDID", SqlDbType.Int)
            {
                Value = _dbUtilities.ToDbValue(saveTheDateId)
            };

            return _dbUtilities.GetDataTable(sql,
                parameter,
                _optionsAggregator.GetCompanyConnectionString(company, year),
                CommandType.Text);
        }

        /// <summary>
        /// Gets adhoc slidekit ids by the specified <paramref name="speakerCounter"/>.
        /// </summary>
        /// <param name="company">The company of the connection string.</param>
        /// <param name="year">The year of the connection string.</param>
        /// <param name="speakerCounter">The speaker counter to search on.</param>
        /// <returns>A <see cref="DataTable"/> containing the adhoc slidekit ids.</returns>
        public DataTable GetAdhocSlidekitIdsBySpeakerCounter(Company company, DocumentYear year, int speakerCounter)
        {
            if (company != Company.AbbottNutrition && company != Company.AbbottNutritionCE)
            {
                return new DataTable();
            }

            const string sql = "usp_GetAdhocSlidekitIdsBySpeakerCounter";
            var parameter = new SqlParameter("SpeakerCounter", SqlDbType.Int)
            {
                Value = _dbUtilities.ToDbValue(speakerCounter)
            };

            return _dbUtilities.GetDataTable(sql,
                parameter,
                _optionsAggregator.GetCompanyConnectionString(company, year),
                CommandType.StoredProcedure);
        }

        /// <summary>
        /// Gets fmvt nomination ids by the specified <paramref name="speakerCounter"/>.
        /// </summary>
        /// <param name="company">The company of the connection string.</param>
        /// <param name="year">The year of the connection string.</param>
        /// <param name="speakerCounter">The speaker counter to search on.</param>
        /// <returns>A <see cref="DataTable"/> containing the fmvt nomination ids.</returns>
        public DataTable GetFMVTNominationIdsBySpeakerCounter(Company company, DocumentYear year, int speakerCounter)
        {
            const string sql = "usp_GetNominationIdsBySpkrCounter";
            var parameter = new SqlParameter("SpeakerCounter", SqlDbType.Int)
            {
                Value = _dbUtilities.ToDbValue(speakerCounter)
            };

            return _dbUtilities.GetDataTable(sql,
                parameter,
                _optionsAggregator.GetCompanyConnectionString(company, year),
                CommandType.StoredProcedure);
        }

        /// <summary>
        /// Gets nomination ids by the specified <paramref name="speakerCounter"/>.
        /// </summary>
        /// <param name="company">The company of the connection string.</param>
        /// <param name="year">The year of the connection string.</param>
        /// <param name="speakerCounter">The speaker counter to search on.</param>
        /// <returns>A <see cref="DataTable"/> containing the nomination ids.</returns>
        public DataTable GetSNNominationIdsBySpeakerCounter(Company company, DocumentYear year, int speakerCounter)
        {
            const string sql = "usp_GetNominationIdsBySpkrCounter";
            var parameter = new SqlParameter("SpeakerCounter", SqlDbType.Int)
            {
                Value = _dbUtilities.ToDbValue(speakerCounter)
            };

            return _dbUtilities.GetDataTable(sql,
                parameter,
                _optionsAggregator.GetCompanyConnectionString(company, year),
                CommandType.StoredProcedure);
        }

        /// <summary>
        /// Gets adhoc slidekit ids by the specified <paramref name="programId"/>.
        /// </summary>
        /// <param name="company">The company of the connection string.</param>
        /// <param name="year">The year of the connection string.</param>
        /// <param name="programId">The program id to search on.</param>
        /// <returns>A <see cref="DataTable"/> containing the adhoc slidekit ids.</returns>
        public DataTable GetAdhocSlidekitIdsByProgramId(Company company, DocumentYear year, string programId)
        {
            if (company != Company.AbbottNutrition && company != Company.AbbottNutritionCE)
            {
                return new DataTable();
            }

            const string sql = "usp_GetAdhocSlidekitIdsByProgramId";
            var parameter = new SqlParameter("ProgramId", SqlDbType.NVarChar)
            {
                Value = _dbUtilities.ToDbValue(programId)
            };

            return _dbUtilities.GetDataTable(sql,
                parameter,
                _optionsAggregator.GetCompanyConnectionString(company, year),
                CommandType.StoredProcedure);
        }

        /// <summary>
        /// Gets slidekit ids by the specified <paramref name="speakerCounter"/>.
        /// </summary>
        /// <param name="company">The company of the connection string.</param>
        /// <param name="year">The year of the connection string.</param>
        /// <param name="speakerCounter">The speaker counter to search on.</param>
        /// <returns>A <see cref="DataTable"/> containing the slidekit ids.</returns>
        public DataTable GetSlideKitIdBySpeakerCounter(Company company, DocumentYear year, int speakerCounter)
        {
            const string sql = "usp_GetSlideKitIdsBySpeakerCounter";
            var parameter = new SqlParameter("SpeakerCounter", SqlDbType.Int)
            {
                Value = _dbUtilities.ToDbValue(speakerCounter)
            };

            return _dbUtilities.GetDataTable(sql,
                parameter,
                _optionsAggregator.GetCompanyConnectionString(company, year),
                CommandType.StoredProcedure);
        }

        /// <summary>
        /// Gets pif ids by the specified <paramref name="programId"/>.
        /// </summary>
        /// <param name="company">The company of the connection string.</param>
        /// <param name="year">The year of the connection string.</param>
        /// <param name="programId">The program id to search on.</param>
        /// <returns>A <see cref="DataTable"/> containing the pif ids.</returns>
        public DataTable GetPifIdsByProgramId(Company company, DocumentYear year, string programId)
        {
            const string sql = "usp_GetPifIdsByProgramId";
            var parameter = new SqlParameter("ProgramId", SqlDbType.VarChar)
            {
                Value = _dbUtilities.ToDbValue(programId)
            };

            return _dbUtilities.GetDataTable(sql,
                parameter,
                _optionsAggregator.GetCompanyConnectionString(company, year),
                CommandType.StoredProcedure);
        }

        /// <summary>
        /// Gets expense counters by the specified <paramref name="vendorId"/>.
        /// </summary>
        /// <param name="company">The company of the connection string.</param>
        /// <param name="year">The year of the connection string.</param>
        /// <param name="vendorId">The vendor id to search on.</param>
        /// <returns>A <see cref="DataTable"/> containing the expense counters.</returns>
        public DataTable GetExpenseCountersByVendorId(Company company, DocumentYear year, int vendorId)
        {
            const string sql = "usp_GetExpenseCountersByVendorId";
            var parameter = new SqlParameter("VendorId", SqlDbType.Int)
            {
                Value = _dbUtilities.ToDbValue(vendorId)
            };

            return _dbUtilities.GetDataTable(sql,
                parameter,
                _optionsAggregator.GetCompanyConnectionString(company, year),
                CommandType.StoredProcedure);
        }

        /// <summary>
        /// Creates and inserts a document log into the database.
        /// </summary>
        /// <param name="documentAcronym">The document acronym.</param>
        /// <param name="fileName">The fileName of the document.</param>
        /// <param name="modifiedDate">The modified date.</param>
        /// <param name="status">The resulting status of the action done to the document.</param>
        /// <param name="actionType">Type of the action done on the document.</param>
        /// <param name="isError">if set to <c>true</c> then the log entry represents an error, else it does not respresent an error.</param>
        /// <param name="message">The message body of the log entry.</param>
        /// <param name="company">The company the document belongs to.</param>
        /// <param name="userName">Name of the user.</param>
        /// <returns>An <see cref="Int32"/> equal to the number of rows affected.</returns>
        public int InsertDocumentLog(string documentAcronym,
                                     string fileName,
                                     DateTime modifiedDate,
                                     SPActionStatus status,
                                     SPDocumentPrivileges actionType,
                                     bool isError,
                                     string message,
                                     Company company,
                                     string userName)
        {
            const string sql = "usp_InsertDocumentLog";
            var parameters = new List<SqlParameter>
                             {
                                 new SqlParameter("DocumentAcronym", SqlDbType.VarChar, 10)
                                 {
                                     Value = _dbUtilities.ToDbValue(documentAcronym)
                                 },
                                 new SqlParameter("Filename", SqlDbType.VarChar, 250)
                                 {
                                     Value = _dbUtilities.ToDbValue(fileName)
                                 },
                                 new SqlParameter("ModifiedDateTime", SqlDbType.DateTime)
                                 {
                                     Value = _dbUtilities.ToDbValue(modifiedDate)
                                 },
                                 new SqlParameter("Modifier", SqlDbType.VarChar, 50)
                                 {
                                     Value = _dbUtilities.ToDbValue(userName)
                                 },
                                 new SqlParameter("IsError", SqlDbType.Bit)
                                 {
                                     Value = _dbUtilities.ToDbValue(isError)
                                 },
                                 new SqlParameter("StatusMessage", SqlDbType.VarChar, 250)
                                 {
                                     Value = _dbUtilities.ToDbValue(message)
                                 },
                                 new SqlParameter("Client", SqlDbType.VarChar, 50)
                                 {
                                     Value = _dbUtilities.ToDbValue(company.ToDisplayNameLong())
                                 }
                             };

            string actionMessage;

            switch (status)
            {
                case SPActionStatus.Failure:
                case SPActionStatus.InvalidFileName:
                    actionMessage = string.Format(Resources.Default.Action_Fail,
                        actionType.ToNameText());
                    break;
                case SPActionStatus.Success:
                    actionMessage = string.Format(Resources.Default.Action_Success,
                        actionType.ToNameText());
                    break;
                default:
                    actionMessage = string.Format(Resources.Default.Action_Unknown_Error,
                        actionType.ToNameText());
                    break;
            }

            parameters.Add(new SqlParameter("Action", SqlDbType.VarChar, 50)
            {
                Value = _dbUtilities.ToDbValue(actionMessage)
            });

            return _dbUtilities.ExecuteNonQuery(sql,
                parameters,
                _options.ConnectionStrings.Event,
                CommandType.StoredProcedure);
        }

        /// <summary>
        /// Inserts a barcode entry into the database.
        /// </summary>
        /// <param name="documentAcronym">The document acronym.</param>
        /// <param name="fileName">Name of the file whose barcode is being inserted.</param>
        /// <param name="company">The company used to determine the database name.</param>
        /// <returns>The barcodeId of the document.</returns>
        public int InsertBarcodeEntry(string documentAcronym, string fileName, Company company)
        {
            string databaseName = string.Empty;
            var barcodeId = 0;

            switch (company)
            {
                case Company.Abbott:
                    databaseName = "Abbott2011";
                    break;
                case Company.AbbottNutrition:
                    databaseName = "Ross2011";
                    break;
                case Company.Solvay:
                    databaseName = "Solvay2011";
                    break;
                case Company.MeiUniversal:
                    databaseName = "MEIUniversal";
                    break;
                case Company.LabDevelopment:
                    databaseName = "Event";
                    break;
                case Company.AbbottNutritionCE:
                    databaseName = "AbbottNutritionCE2011";
                    break;
                case Company.Kowa:
                    databaseName = "Kowa2011";
                    break;
                case Company.LipoScience:
                    databaseName = "LipoScience2011";
                    break;
                case Company.AbbottAnimalHealth:
                    databaseName = "FMV2012";
                    break;
                case Company.AbbottDiabetesCare:
                    databaseName = "FMV2012";
                    break;
                case Company.AbbottDiagnosticsDivision:
                    databaseName = "FMV2012";
                    break;
                case Company.AbbottMolecular:
                    databaseName = "FMV2012";
                    break;
                case Company.AbbottMedicalOptics:
                    databaseName = "FMV2012";
                    break;
                case Company.AbbottPointOfCare:
                    databaseName = "FMV2012";
                    break;
                case Company.AbbottVascular:
                    databaseName = "FMV2012";
                    break;
                case Company.Corporate:
                    databaseName = "FMV2012";
                    break;
                case Company.EstablishedProductsDivision:
                    databaseName = "FMV2012";
                    break;
                case Company.GlobalPharmaceuticalResearchAndDevelopment:
                    databaseName = "FMV2012";
                    break;
                case Company.GlobalStrategicMarketingAndServices:
                    databaseName = "FMV2012";
                    break;
                case Company.ProprietaryPharmaceuticalsDivision:
                    databaseName = "FMV2012";
                    break;
                case Company.PharmaseuticalProductsGroup:
                    databaseName = "FMV2012";
                    break;
                case Company.RegulatoryAffairsPPG:
                    databaseName = "FMV2012";
                    break;
            }

            const string sql = "usp_InsertBarcodeEntry";
            var parameters = new List<SqlParameter>
                             {
                                 new SqlParameter("DocumentAcronym", SqlDbType.VarChar, 10)
                                 {
                                     Value = _dbUtilities.ToDbValue(documentAcronym)
                                 },
                                 new SqlParameter("Filename", SqlDbType.VarChar, 256)
                                 {
                                     Value = _dbUtilities.ToDbValue(fileName)
                                 },
                                 new SqlParameter("Database", SqlDbType.VarChar, 50)
                                 {
                                     Value = _dbUtilities.ToDbValue(databaseName)
                                 },
                                 new SqlParameter("BarcodeID", SqlDbType.Int)
                                 {
                                     Direction = ParameterDirection.Output
                                 }
                             };

            _dbUtilities.ExecuteNonQuery(sql,
                parameters,
                _options.ConnectionStrings.Event,
                CommandType.StoredProcedure);

            foreach (SqlParameter p in parameters)
            {
                if (p.ParameterName == "BarcodeID")
                {
                    barcodeId = _dbUtilities.FromDbValue<int>(p.Value);
                    break; // TODO: might not be correct. Was : Exit For
                }
            }

            return barcodeId;
        }

        /// <summary>
        /// Gets the user's document privileges from the database.
        /// </summary>
        /// <param name="documentType">Type of the document to get the privileges for.  f none, then the privileges for
        /// all document types will be returned.</param>
        /// <param name="domainGroupsForUser">The domain groups for user.</param>
        /// <returns>A DocumentPrivilegesCollection containing the privileges of the specified domain groups.</returns>
        public IList<DocumentAccessInfo> GetDocumentAccessInfos(SPDocumentType documentType, IList<string> domainGroupsForUser)
        {
            var basePrivileges = new List<DocumentAccessInfo>();
            string documentAcronym = string.Empty;

            if (documentType != SPDocumentType.None)
            {
                documentAcronym = _documentInfoAggregator.CodeToAcronym(documentType);
            }

            const string sql = "usp_GetAllUserDocumentPrivileges";
            var parameters = new List<SqlParameter>
                             {
                                 new SqlParameter("UserGroups", SqlDbType.VarChar)
                                 {
                                     Value = _dbUtilities.ToDbValue(string.Join(",", domainGroupsForUser.ToArray()))
                                 }
                             };

            if (documentType != SPDocumentType.None)
            {
                parameters.Add(new SqlParameter("DocumentAcronym", SqlDbType.VarChar)
                {
                    Value = _dbUtilities.ToDbValue(documentAcronym)
                });
            }

            using (SqlDataReader rdr = _dbUtilities.ExecuteDataReader(sql,
                parameters,
                _options.ConnectionStrings.Event,
                CommandType.StoredProcedure))
            {
                while (rdr.Read())
                {
                    SPDocumentType thisDocumentType;

                    try
                    {
                        thisDocumentType = _documentInfoAggregator.AcronymToCode(_dbUtilities.FromDbValue<string>(rdr["DocumentAcronym"]));
                    }
                    catch (ArgumentException ex)
                    {
                        if (ex.Message.Contains("Invalid acronym") && ex.ParamName == "acronym")
                        {
                            //TODO: send error email
                            continue;
                        }

                        throw;
                    }

                    SPDocumentPrivileges thisPrivilege =
                        _dbUtilities.FromDbValue<string>(rdr["Privilege"]).ToDocumentPrivileges();
                    basePrivileges.Add(new DocumentAccessInfo(thisDocumentType,
                        thisPrivilege,
                        _dbUtilities.FromDbValue<string>(rdr["GroupName"]),
                        _dbUtilities.FromDbValue<int>(rdr["Importance"])));
                }
            }

            return basePrivileges;
        }

        public int CreateTicklersOnSignInSheetUpload(string programId, Company company, DocumentYear year)
        {
            const string sql = "usp_CreateTicklersOnSigninShUploaded";
            var parameters = new List<SqlParameter>
                             {
                                 new SqlParameter("ProgramID", SqlDbType.VarChar, 20)
                                 {
                                     Value = _dbUtilities.ToDbValue(programId)
                                 }
                             };

            return _dbUtilities.ExecuteNonQuery(sql,
                parameters,
                _optionsAggregator.GetCompanyConnectionString(company, year),
                CommandType.StoredProcedure);
        }

        public int CreateTicklersOnProgramAttestationUpload(string programId, Company company, DocumentYear year)
        {
            const string sql = "AutoCloseOutQsCompleteDuopaAEProgram";
            var parameters = new List<SqlParameter>
                             {
                                 new SqlParameter("ProgramID", SqlDbType.VarChar, 20)
                                 {
                                     Value = _dbUtilities.ToDbValue(programId)
                                 }
                             };

            return _dbUtilities.ExecuteNonQuery(sql,
                parameters,
                _optionsAggregator.GetCompanyConnectionString(company, year),
                CommandType.StoredProcedure);
        }

        public int InsertIntoReceiptsOnReceiptUpload(string programId, int? expenseCounter, Company company, DocumentYear year)
        {
            var parameters = new List<SqlParameter>
                          {
                              new SqlParameter("@ProgramID", SqlDbType.VarChar, 50)
                              {
                                  Value = _dbUtilities.ToDbValue(programId)
                              },
                              new SqlParameter("@ExpenseCounter", SqlDbType.Int)
                              {
                                  Value = _dbUtilities.ToDbValue(expenseCounter)
                              }
                          };

            parameters.Add(new SqlParameter("@RctShareID", SqlDbType.VarChar, 50)
            {
                Value = _dbUtilities.ToDbValue(parameters[0].Value + parameters[1].Value.ToString())
            });

            return _dbUtilities.ExecuteNonQuery("usp_InsertIntoReceipts",
                parameters,
                _optionsAggregator.GetCompanyConnectionString(company, year),
                CommandType.StoredProcedure);
        }

        public int UpdateSpeakerBuiltSlideKitOnUpload(string programId, int speakerCounter, DocumentYear year)
        {
            const string sql = "Update SpeakerBuiltSlideKitInfo Set Processed = 1, ProcessDateTime = @ProcessDateTime Where ProgramId = @ProgramID and SpeakerCounter = @SpeakerCounter and Ignore <> 1";
            var parameters = new List<SqlParameter>
                          {
                              new SqlParameter("@ProcessDateTime", SqlDbType.DateTimeOffset)
                              {
                                  Value = _dbUtilities.ToDbValue(DateTimeOffset.Now)
                              },
                              new SqlParameter("@ProgramID", SqlDbType.VarChar, 50)
                              {
                                  Value = _dbUtilities.ToDbValue(programId)
                              },
                              new SqlParameter("@SpeakerCounter", SqlDbType.Int)
                              {
                                  Value = _dbUtilities.ToDbValue(speakerCounter)
                              }
                          };

            return _dbUtilities.ExecuteNonQuery(sql,
                parameters,
                _optionsAggregator.GetCompanyConnectionString(Company.AbbottNutrition, year),
                CommandType.Text);
        }

        public int GetSequenceNumberForAttendeeProgramEvaluation(Company company,
                                                                 DocumentYear year,
                                                                 string programId,
                                                                 int pageCount)
        {
            const string sql = "InsertNewSpeakerEvalToSP";
            var parameters = new List<SqlParameter>
                             {
                                 new SqlParameter("ProgramID", SqlDbType.VarChar, 50)
                                 {
                                     Value = _dbUtilities.ToDbValue(programId)
                                 },
                                 new SqlParameter("PageCount", SqlDbType.Int)
                                 {
                                     Value = _dbUtilities.ToDbValue(pageCount)
                                 }
                             };
            var outParameter = new SqlParameter("Sequence", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            parameters.Add(outParameter);

            _dbUtilities.ExecuteNonQuery(sql,
                parameters,
                _optionsAggregator.GetCompanyConnectionString(company, year),
                CommandType.StoredProcedure);
            if (outParameter.Value == DBNull.Value)
            {
                throw new ApplicationException(string.Format("procedure did not return a sequence number. {0}", programId));
            }

            return (int)outParameter.Value;
        }

        public DataTable GetSecondAddressAmendmentProofById(Company company, DocumentYear year, int secondAddressAmendmentId)
        {
            var parameters = new List<SqlParameter>
                     {
                         new SqlParameter("@mspaSecondAddrAmendmentID", SqlDbType.Int)
                         {
                             Value = _dbUtilities.ToDbValue(Convert.ToInt32(secondAddressAmendmentId))
                         }
                     };
            DataTable dt = _dbUtilities.GetDataTable("GetMSPASecondAddrAmendmentProofById",
                parameters,
                _optionsAggregator.GetCompanyConnectionString(company, year),
                CommandType.StoredProcedure);

            return dt;
        }

        public void SendInviteApprovalEmail(Company company, DocumentYear year, string programId)
        {
            const string sql = "usp_SendInviteApprovalEmail";
            var parameters = new List<SqlParameter>
                             {
                                 new SqlParameter("@programid", SqlDbType.VarChar)
                                 {
                                     Value = _dbUtilities.ToDbValue(programId)
                                 }
                             };

            _dbUtilities.ExecuteNonQuery(sql,
                parameters,
                _optionsAggregator.GetCompanyConnectionString(company, year),
                CommandType.StoredProcedure);
        }

        public string GetInvoiceNumberForTravelDocId(int travelDocId)
        {
            const string sql = "SELECT InvoiceNumber FROM dbo.TravelDocSearch WHERE Id = @TravelDocId;";
            var parameter = new SqlParameter("TravelDocId", SqlDbType.Int)
            {
                Value = _dbUtilities.ToDbValue(travelDocId)
            };

            DataTable dt = _dbUtilities.GetDataTable(sql,
                parameter,
                ConfigurationManager.ConnectionStrings["Event"].ConnectionString,
                CommandType.Text);

            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0]["InvoiceNumber"].ToString();
            }

            return string.Empty;
        }

        public string GetTicketNumberForTravelDocId(int travelDocId)
        {
            const string sql = "SELECT TicketNumber FROM dbo.TravelDocSearch WHERE Id = @TravelDocId;";
            var parameter = new SqlParameter("TravelDocId", SqlDbType.Int)
            {
                Value = _dbUtilities.ToDbValue(travelDocId)
            };

            DataTable dt = _dbUtilities.GetDataTable(sql,
                parameter,
                ConfigurationManager.ConnectionStrings["Event"].ConnectionString,
                CommandType.Text);

            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0]["TicketNumber"].ToString();
            }

            return string.Empty;
        }

        public string GetRecordLocatorForTravelDocId(int travelDocId)
        {
            const string sql = "SELECT RecordLocator FROM dbo.TravelDocSearch WHERE Id = @TravelDocId;";
            var parameter = new SqlParameter("TravelDocId", SqlDbType.Int)
            {
                Value = _dbUtilities.ToDbValue(travelDocId)
            };

            DataTable dt = _dbUtilities.GetDataTable(sql,
                parameter,
                ConfigurationManager.ConnectionStrings["Event"].ConnectionString,
                CommandType.Text);

            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0]["RecordLocator"].ToString();
            }

            return string.Empty;
        }

        public virtual string[] GetApprovedDocumentDeletes()
        {
            const string sql = "Select * From SPDeleteQueue Where ReviewAction = 'Approved'";
            return _dbUtilities.GetColumnValues<string>(sql,
                "FileName",
                (List<SqlParameter>)null,
                _options.ConnectionStrings.Event,
                CommandType.Text);
        }
    }

    internal class CachedRepository
        : Repository, ICachedRepository
    {
        private static readonly object LockForDeleteQueue = new object();

        public CachedRepository(IDbUtilities dbUtilities,
                                IOptions<SPDocumentsOptions> options,
                                ISPDocumentsOptionsAggregator optionsAggregator,
                                IDocumentInfoAggregator documentInfoAggregator,
                                ICacheProvider cacheProvider)
            : base(dbUtilities, options, optionsAggregator, documentInfoAggregator)
        {
            CacheProvider = cacheProvider;
        }

        public ICacheProvider CacheProvider { get; }

        public override string[] GetApprovedDocumentDeletes()
        {
            return CacheProvider.GetCachedData("GetApprovedDocumentDeletes",
                LockForDeleteQueue,
                5,
                () => base.GetApprovedDocumentDeletes());
        }
    }
}
