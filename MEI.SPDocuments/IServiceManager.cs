using System;
using System.Collections.Generic;
using System.Linq;

using MEI.SPDocuments.Services;
using MEI.SPDocuments.SPActionResult;
using MEI.SPDocuments.TypeCodes;

using Microsoft.Extensions.Logging;
using Microsoft.SharePoint.Client;

namespace MEI.SPDocuments
{
    public interface IServiceManager
        : IDisposable
    {
        IList<SearchResult> SearchDocument(Company company,
                                           string documentLibraryTitle,
                                           string camlQuery,
                                           string baseSiteUrl,
                                           SPFieldCollection spFields,
                                           bool includeVersions);

        UploadResult UploadDocument(Company company,
                                    string baseSiteUrl,
                                    string documentLibraryTitle,
                                    string fileName,
                                    byte[] fileContents,
                                    IDictionary<string, string> userFields);
    }

    /// <summary>
    ///     Provides procedures to call SharePoint web service methods on documents.
    /// </summary>
    internal class SPServiceManager
        : IServiceManager
    {
        private readonly ServiceCache _serviceCache;
        private readonly ILogger<SPServiceManager> _logger;
        private readonly IEncryptor _encryptor;

        public SPServiceManager(ServiceCache serviceCache,
                                ILogger<SPServiceManager> logger,
                                IEncryptor encryptor)
        {
            _serviceCache = serviceCache;
            _logger = logger;
            _encryptor = encryptor;
        }

        public IList<SearchResult> SearchDocument(Company company,
                                                  string documentLibraryTitle,
                                                  string camlQuery,
                                                  string baseSiteUrl,
                                                  SPFieldCollection spFields,
                                                  bool includeVersions)
        {
            Preconditions.CheckNotNullOrEmpty(nameof(baseSiteUrl), baseSiteUrl);
            Preconditions.CheckNotNull(nameof(spFields), spFields);

            DocumentContext service = _serviceCache[company];

            IList<File> items = service.Search(documentLibraryTitle, camlQuery, includeVersions);

            if ((items == null) || (items.Count <= 0))
            {
                return new List<SearchResult>();
            }

            return items.Select(x => new SearchResult(_encryptor, x, spFields, baseSiteUrl)).ToList();
        }

        public UploadResult UploadDocument(Company company,
                                           string baseSiteUrl,
                                           string documentLibraryTitle,
                                           string fileName,
                                           byte[] fileContents,
                                           IDictionary<string, string> userFields)
        {
            Preconditions.CheckNotNullOrEmpty(nameof(baseSiteUrl), baseSiteUrl);
            Preconditions.CheckNotNullOrEmpty(nameof(documentLibraryTitle), documentLibraryTitle);
            Preconditions.CheckNotNullOrEmpty(nameof(fileName), fileName);
            Preconditions.CheckNotNull(nameof(fileContents), fileContents);

            DocumentContext service = _serviceCache[company];

            service.Upload(documentLibraryTitle, fileName, fileContents, userFields);

            return new UploadResult(SPActionStatus.Success, "Success Upload");
        }

        public void Dispose()
        {
            _serviceCache?.Dispose();
        }
    }
}
