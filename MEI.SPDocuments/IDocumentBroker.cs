using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using MEI.SPDocuments.Data;
using MEI.SPDocuments.Document;
using MEI.SPDocuments.Security;
using MEI.SPDocuments.SPActionResult;
using MEI.SPDocuments.TypeCodes;

using MEI.Core.Infrastructure.Services;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using NodaTime;
using NodaTime.Extensions;

namespace MEI.SPDocuments
{
    /// <summary>
    ///     Provides procedures for interaction with documents.
    /// </summary>
    public interface IDocumentBroker
        : IDisposable
    {
        /// <summary>
        ///     Gets or sets a value indicating whether this instance validates document fields.
        /// </summary>

        bool IsValidateFields { get; set; }
        /// <summary>
        ///     Gets the document access control object.
        /// </summary>

        IDocumentAccessControl DocumentAccessControl { get; }

        /// <summary>
        ///     Uploads the specified <paramref name="doc" />.
        /// </summary>
        /// <param name="doc">The document to upload.</param>
        /// <returns>An <see cref="UploadResult" /> containing the results of the upload attempt.</returns>
        UploadResult Upload(IDocument doc);

        (bool success, string message) SaveWebDocumentToWebShare(IDocument doc);

        /// <summary>
        ///     Searches for documents.
        /// </summary>
        /// <param name="documentType">Type of the document.</param>
        /// <param name="company">The company the documents belong to.</param>
        /// <param name="searchExpressions">The search expressions.</param>
        /// <returns>An IList of <see cref="SearchResult" /> containing the results of the search attempt.</returns>
        IList<SearchResult> Search(SPDocumentType documentType, Company company, ICompleteSearchExpressionGroup searchExpressions);

        byte[] RetrieveFile(SPDocumentType documentType, SearchResult searchResult);

        byte[] RetrieveFile(SPDocumentType documentType, string encryptedUrl);
    }

    /// <summary>
    ///     Provides procedures for interaction with the SharePoint documents.
    /// </summary>
    internal class DocumentBroker
        : IDocumentBroker
    {
        private readonly IDocumentFactory _documentFactory;
        private readonly IServiceManager _serviceManager;
        private readonly ILogger<DocumentBroker> _logger;
        private readonly IEncryptor _encryptor;
        private readonly IPdfTools _pdfTools;
        private readonly SPDocumentsOptions _options;
        private readonly IRepository _repository;
        private readonly IClock _clock;
        private readonly IDocumentInfoAggregator _documentInfoAggregator;
        private readonly ISPDocumentsOptionsAggregator _optionsAggregator;
        private readonly IFileDownloader _fileDownloader;
        private readonly IUserResolverService _userNameResolver;

        public DocumentBroker(IDocumentFactory documentFactory,
                                IServiceManager serviceManager,
                                ILogger<DocumentBroker> logger,
                                IDocumentAccessControl documentAccessControl,
                                IEncryptor encryptor,
                                IPdfTools pdfTools,
                                IOptions<SPDocumentsOptions> options,
                                IRepository repository,
                                IClock clock,
                                IDocumentInfoAggregator documentInfoAggregator,
                                ISPDocumentsOptionsAggregator optionsAggregator,
                                IFileDownloader fileDownloader,
                                IUserResolverService userNameResolver)
        {
            IsValidateFields = true;

            _documentFactory = documentFactory;
            _serviceManager = serviceManager;
            _logger = logger;
            DocumentAccessControl = documentAccessControl;
            _encryptor = encryptor;
            _pdfTools = pdfTools;
            _options = options.Value;
            _repository = repository;
            _clock = clock;
            _documentInfoAggregator = documentInfoAggregator;
            _optionsAggregator = optionsAggregator;
            _fileDownloader = fileDownloader;
            _userNameResolver = userNameResolver;
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance validates document fields.
        /// </summary>
        public bool IsValidateFields { get; set; }

        /// <summary>
        ///     Gets the document access control object associated with this instance.
        /// </summary>
        public IDocumentAccessControl DocumentAccessControl { get; }

        /// <summary>
        ///     Gets the current userName.
        /// </summary>
        public string UserName => _userNameResolver.GetUserName();

        /// <summary>
        ///     Uploads the specified <paramref name="doc" /> to SharePoint.
        /// </summary>
        /// <param name="doc">The document to upload.</param>
        /// <returns>An <see cref="UploadResult" /> containing the results of the upload attempt.</returns>
        public UploadResult Upload(IDocument doc)
        {
            Preconditions.CheckNotNull("doc", doc);

            UploadResult uploadResult;

            try
            {
                var uploadAction = (IUploadAction)doc;
                uploadAction.ActionBeforeUpload(this);

                if (IsValidateFields && !doc.ValidateFields())
                {
                    uploadResult = new UploadResult(SPActionStatus.InvalidFileName,
                        string.Format(Resources.Default.Validation_Failed_0, doc.OriginalFileName));

                    CreateDocumentLog(_documentInfoAggregator.AcronymToCode(doc.Acronym),
                        doc.OriginalFileName,
                        uploadResult.Status,
                        true,
                        uploadResult.Message,
                        doc.Company,
                        SPDocumentPrivileges.Upload);

                    return uploadResult;
                }

                string baseSiteUrl = _options.BaseSiteUrl;
                string siteRelativeUrl = _optionsAggregator.GetSiteRelativeUrl(doc.Company);
                string documentLibraryTitle = _optionsAggregator.GetDocumentLibraryTitle(doc.Company, doc.DocumentType);

                IDictionary<string, string> fields = doc.GetUserFieldValues();

                fields.Add("Title", doc.FileName);

                uploadResult = _serviceManager.UploadDocument(doc.Company,
                    baseSiteUrl + siteRelativeUrl,
                    documentLibraryTitle,
                    doc.FileName,
                    doc.Contents,
                    fields);

                CreateDocumentLog(doc.DocumentType,
                    doc.FileName,
                    uploadResult.Status,
                    false,
                    uploadResult.Message,
                    doc.Company,
                    SPDocumentPrivileges.Upload);

                uploadAction.ActionAfterUpload(uploadResult);
            }
            catch (Exception ex)
            {
                uploadResult = new UploadResult(SPActionStatus.Failure, ex);

                CreateDocumentLog(doc.DocumentType,
                    doc.FileName,
                    uploadResult.Status,
                    true,
                    uploadResult.Message,
                    doc.Company,
                    SPDocumentPrivileges.Upload);

                if (doc is IUploadAction uploadAction)
                {
                    uploadAction.ActionAfterUpload(uploadResult);
                }
            }

            return uploadResult;
        }

        public (bool success, string message) SaveWebDocumentToWebShare(IDocument doc)
        {
            Preconditions.CheckNotNull("doc", doc);

            string webPath;

            try
            {
                webPath = _documentInfoAggregator.DocumentTypeInfos[doc.DocumentType].WebDocumentPath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);

                return (false, Resources.Default.Unable_to_get_web_path_of_document);
            }

            string docWebPath;

            try
            {
                string documentFileName = doc.GetParsableFileNameWithCompanyYearAndxIndex(doc.Company, doc.DocumentYear);

                if (documentFileName.Split(new[] { "__" }, StringSplitOptions.RemoveEmptyEntries).Length == 2)
                {
                    return (false, Resources.Default.Filename_was_invalid);
                }

                docWebPath = string.Format("{0}{1}.{2}", webPath, documentFileName, doc.FileExtension);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);

                docWebPath = string.Format("{0}{1}({2}).{3}",
                    webPath,
                    doc.GetParsableFileNameWithCompanyYear(doc.Company, doc.DocumentYear),
                    Guid.NewGuid().ToString(),
                    doc.FileExtension);
            }

            try
            {
                File.WriteAllBytes(docWebPath, doc.Contents);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);

                CreateDocumentLog(doc.DocumentType,
                    doc.FileName,
                    SPActionStatus.Failure,
                    true,
                    ex.ToString(),
                    doc.Company,
                    SPDocumentPrivileges.Save);

                return (false, Resources.Default.File_was_not_saved_correctly);
            }

            if (!File.Exists(docWebPath))
            {
                CreateDocumentLog(doc.DocumentType,
                    doc.FileName,
                    SPActionStatus.Failure,
                    true,
                    "File was not saved correctly.",
                    doc.Company,
                    SPDocumentPrivileges.Save);

                return (false, Resources.Default.File_was_not_saved_correctly);
            }

            CreateDocumentLog(doc.DocumentType,
                doc.FileName,
                SPActionStatus.Success,
                false,
                "File was written to the web share.",
                doc.Company,
                SPDocumentPrivileges.Save);

            return (true, Resources.Default.File_was_saved_correctly);
        }

        /// <summary>
        ///     Searches for documents in SharePoint.
        /// </summary>
        /// <param name="documentType">Type of the document.</param>
        /// <param name="company">The company the documents belong to.</param>
        /// <param name="searchExpressions">The search expressions.</param>
        /// <returns>An IList of <see cref="SearchResult" /> containing the results of the search attempt.</returns>
        /// <example>
        ///     <code lang="C#">
        /// // the document type to search for
        /// const SPDocumentTypeCode documentType = SPDocumentTypeCode.BanquetEventOrder;
        /// 
        /// // the company whose document library to search in
        /// const CompanyCode company = CompanyCode.Abbott;
        /// 
        /// // a SearchExpressionGroup containing the fields to search on
        /// // in this case, a single field, which is ProgramId and whose value is Equal to 00000-00-00
        /// var searchExpressionGroup = new SearchExpressionGroup(SPDocumentTypeCode.BanquetEventOrder, SearchBooleanLogicCode.And);
        /// searchExpressionGroup.AddExpression((int)BanquetEventOrderFieldName.ProgramId, CamlComparisonCode.Equal, "00000-00-00");
        /// 
        /// // the call to Search with the above values
        /// // storing the results in searchResults for later processing
        /// var searchResults = _spDocs.DocBroker.Search(documentType, company, searchExpressionGroup);
        /// </code>
        /// </example>
        public IList<SearchResult> Search(SPDocumentType documentType, Company company, ICompleteSearchExpressionGroup searchExpressions)
        {
            Preconditions.CheckNotNull("searchExpressions", searchExpressions);

            IDocument doc = _documentFactory.CreateDocument(documentType);

            return Search(doc, company, searchExpressions.MakeQuery());
        }

        public byte[] RetrieveFile(SPDocumentType documentType, string encryptedUrl)
        {
            Preconditions.CheckNotNullOrEmpty("encryptedUrl", encryptedUrl);

            string decryptedUrl = _encryptor.Decrypt(encryptedUrl);

            return RetrieveFileByUrl(decryptedUrl, documentType);
        }

        public byte[] RetrieveFile(SPDocumentType documentType, SearchResult searchResult)
        {
            Preconditions.CheckNotNull("searchResult", searchResult);

            var decryptedSearchResult = new DecryptedSearchResult(_encryptor, searchResult);

            return RetrieveFileByUrl(decryptedSearchResult.DecryptedDocumentAbsoluteUrl, documentType);
        }

        private IList<SearchResult> Search(IDocument doc, Company company, string query)
        {
            Preconditions.CheckNotNull("doc", doc);
            Preconditions.CheckEnum("company", company, Company.Undefined);
            Preconditions.CheckNotNullOrEmpty("query", query);

            if (!_optionsAggregator.IsEnabled(company, doc.DocumentType))
            {
                return new List<SearchResult>();
            }

            string documentLibraryTitle = _optionsAggregator.GetDocumentLibraryTitle(company, doc.DocumentType);
            string baseSiteUrl = _options.BaseSiteUrl;
            string siteRelativeUrl = _optionsAggregator.GetSiteRelativeUrl(company);

            if (!DocumentAccessControl.HasPrivilege(UserName, doc.DocumentType, SPDocumentPrivileges.Search))
            {
                CreatePermissionDeniedLog(doc.DocumentType, company, SPDocumentPrivileges.Search);

                return new List<SearchResult>();
            }

            bool userCanSearchVersions = DocumentAccessControl.HasPrivilege(UserName, doc.DocumentType, SPDocumentPrivileges.SearchVersions);

            IList<SearchResult> searchResults = _serviceManager.SearchDocument(company,
                documentLibraryTitle,
                string.Format("<View><Query>{0}</Query></View>", query),
                baseSiteUrl + siteRelativeUrl,
                doc.SPFields,
                userCanSearchVersions);

            //' **** NOTE: THIS IS A Workaround - Remove once travel doc libraries are completed *** ''''
            if (doc.DocumentType == SPDocumentType.TravelReceipt)
            {
                const string oldDocumentLibraryTitle = "Travel Receipt";
                IList<SearchResult> oldTravelDocResults = _serviceManager.SearchDocument(company,
                    oldDocumentLibraryTitle,
                    query,
                    baseSiteUrl + siteRelativeUrl,
                    doc.SPFields,
                    userCanSearchVersions);

                foreach (SearchResult searchResult in oldTravelDocResults)
                {
                    searchResults.Add(searchResult);
                }
            }
            //' **** NOTE: THIS IS A Workaround - Remove once travel doc libraries are completed *** ''''

            string[] dt = _repository.GetApprovedDocumentDeletes();

            for (var i = 0; i <= searchResults.Count - 1; i++)
            {
                string thisFileName = "v" + searchResults[i].Version + " - " + searchResults[i].DocumentName;

                if (dt.Contains(thisFileName))
                {
                    searchResults[i].IsDisabled = true;
                }

                if (searchResults[i].Versions!= null)
                {
                    foreach (SearchVersionsResult item in searchResults[i].Versions)
                    {
                        if (dt.Contains(item.DisplayName))
                        {
                            item.IsDisabled = true;
                        }

                        searchResults[i].Versions.Add(item);
                    }
                }
            }

            return searchResults;
        }

        private void CreatePermissionDeniedLog(SPDocumentType documentType, Company company, SPDocumentPrivileges action)
        {
            CreateDocumentLog(documentType,
                "",
                SPActionStatus.Failure,
                false,
                string.Format(Resources.Default.Permission_denied, UserName),
                company,
                action);
        }

        private byte[] RetrieveFileByUrl(string decryptedUrl, SPDocumentType documentType)
        {
            Preconditions.CheckNotNullOrEmpty("url", decryptedUrl);
            Preconditions.CheckEnum("documentType", documentType, SPDocumentType.None);

            if (!DocumentAccessControl.HasPrivilege(UserName, documentType, SPDocumentPrivileges.ViewOriginal)
                && !DocumentAccessControl.HasPrivilege(UserName, documentType, SPDocumentPrivileges.ViewWatermarked))
            {
                CreatePermissionDeniedLog(documentType,
                    Company.Undefined,
                    SPDocumentPrivileges.ViewOriginal | SPDocumentPrivileges.ViewWatermarked);

                return null;
            }

            var url = new Uri(decryptedUrl);
            byte[] fileBytes = _fileDownloader.DownloadFile(_options.CredentialUserName,
                _options.CredentialPassword,
                _options.CredentialDomain,
                url);

            if (DocumentAccessControl.HasPrivilege(UserName, documentType, SPDocumentPrivileges.ViewWatermarked))
            {
                IDocument doc;
                string connectionString;

                try
                {
                    Company company = _options.GetCompanyByUrl(decryptedUrl.Split(Convert.ToChar("/"))[2]).ToCompany();
                    string fileName = decryptedUrl.Split(Convert.ToChar("/"))[decryptedUrl.Split(Convert.ToChar("/")).Length - 1];

                    doc = _documentFactory.CreateDocument(documentType, company, fileName, fileBytes);
                    connectionString = _optionsAggregator.GetCompanyConnectionString(company, doc.DocumentYear);
                }
                catch
                {
                    doc = _documentFactory.CreateDocument(documentType);
                    connectionString = string.Empty;
                }

                WatermarkProfile docWatermarkProfile = doc.GetWaterMarkProfile(connectionString);

                if (docWatermarkProfile != null)
                {
                    fileBytes = _pdfTools.WatermarkDocument(fileBytes, docWatermarkProfile);
                }
                else
                {
                    fileBytes = _pdfTools.WatermarkDocument(fileBytes, 2, 5, "VOID");
                }
            }

            if ((fileBytes == null) || (fileBytes.Length == 0))
            {
                throw new InvalidDataException(string.Format(Resources.Default.Could_not_retrieve_document_at_Url__0,
                    url.AbsoluteUri,
                    _documentInfoAggregator.CodeToAcronym(documentType),
                    UserName));
            }

            return fileBytes;
        }

        private void CreateDocumentLog(SPDocumentType documentType,
                             string fileName,
                             SPActionStatus status,
                             bool isError,
                             string message,
                             Company company,
                             SPDocumentPrivileges actionType)
        {
            _repository.InsertDocumentLog(_documentInfoAggregator.CodeToAcronym(documentType),
                fileName,
                _clock.InTzdbSystemDefaultZone().GetCurrentLocalDateTime().ToDateTimeUnspecified(),
                status,
                actionType,
                isError,
                message,
                company,
                UserName);
        }

        public void Dispose()
        {
            _serviceManager?.Dispose();
        }
    }
}
