using System;
using System.Collections.Generic;

using MEI.SPDocuments.Document;
using MEI.SPDocuments.Security;
using MEI.SPDocuments.SPActionResult;
using MEI.SPDocuments.TypeCodes;

namespace MEI.SPDocuments
{
    public interface ISPDocuments
        : IDisposable
    {
        /// <summary>
        ///     Gets the document access control used by this instance.
        /// </summary>
        IDocumentAccessControl DocumentAccessControl { get; }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance will validate fields.
        /// </summary>
        bool IsValidateFields { get; set; }

        /// <summary>
        ///     Uploads a document.
        /// </summary>
        /// <param name="doc">The document to upload.</param>
        /// <returns>An <see cref="UploadResult" /> containing the results of the upload action.</returns>
        UploadResult Upload(IDocument doc);

        byte[] RetrieveFile(SPDocumentType documentType, string encryptedUrl);

        byte[] RetrieveFile(SPDocumentType documentType, SearchResult searchResult);

        IList<SearchResult> Search(SPDocumentType documentType, Company company, ICompleteSearchExpressionGroup searchExpressions);

        string GetCompanyConnectionString(Company company, DocumentYear year);

        SortedList<string, string> GetCompanyYears(Company company);

        (bool success, string message) SaveWebDocumentToWebShare(IDocument doc);

        ISearchExpressionGroup CreateSearchExpression(SPDocumentType documentType, SPFieldNames enumValue, CamlComparison comparison, int? value);

        ISearchExpressionGroup CreateSearchExpression(SPDocumentType documentType, SPFieldNames enumValue, CamlComparison comparison, long? value);

        ISearchExpressionGroup CreateSearchExpression(SPDocumentType documentType, SPFieldNames enumValue, CamlComparison comparison, string value);

        ISearchExpressionGroup CreateSearchExpression(SPDocumentType documentType, SPFieldNames enumValue, CamlComparison comparison, DateTime? value);
    }

    /// <summary>
    ///     Provides general operations on all aspects of a document.
    /// </summary>
    /// <remarks>
    ///     An instance of this class is generally what most referencing assemblies will use in regards
    ///     this class library.
    /// </remarks>
    public class SP_Documents
        : ISPDocuments
    {
        private readonly IDocumentBroker _docBroker;
        private readonly ISPDocumentsOptionsAggregator _optionsAggregator;
        private readonly IDocumentFactory _documentFactory;

        /// <summary>
        ///     Initializes a new instance of the <see cref="SP_Documents" /> class.
        /// </summary>
        /// <param name="documentBroker">The document broker for this instance to use.</param>
        /// <param name="optionsAggregator"></param>
        /// <param name="documentFactory"></param>
        public SP_Documents(IDocumentBroker documentBroker, ISPDocumentsOptionsAggregator optionsAggregator, IDocumentFactory documentFactory)
        {
            _docBroker = documentBroker;
            _optionsAggregator = optionsAggregator;
            _documentFactory = documentFactory;
        }

        /// <summary>
        ///     Gets the document access control used by this instance.
        /// </summary>
        public IDocumentAccessControl DocumentAccessControl => _docBroker.DocumentAccessControl;

        /// <summary>
        ///     Gets or sets a value indicating whether this instance will validate fields.
        /// </summary>
        public bool IsValidateFields
        {
            get => _docBroker.IsValidateFields;

            set => _docBroker.IsValidateFields = value;
        }

        /// <summary>
        ///     Uploads a document.
        /// </summary>
        /// <param name="doc">The document to upload.</param>
        /// <returns>An <see cref="UploadResult" /> containing the results of the upload action.</returns>
        public UploadResult Upload(IDocument doc)
        {
            return _docBroker.Upload(doc);
        }

        public byte[] RetrieveFile(SPDocumentType documentType, string encryptedUrl)
        {
            return _docBroker.RetrieveFile(documentType, encryptedUrl);
        }

        public byte[] RetrieveFile(SPDocumentType documentType, SearchResult searchResult)
        {
            return _docBroker.RetrieveFile(documentType, searchResult);
        }

        public IList<SearchResult> Search(SPDocumentType documentType, Company company, ICompleteSearchExpressionGroup searchExpressions)
        {
            return _docBroker.Search(documentType, company, searchExpressions);
        }

        public string GetCompanyConnectionString(Company company, DocumentYear year)
        {
            return _optionsAggregator.GetCompanyConnectionString(company, year);
        }

        public SortedList<string, string> GetCompanyYears(Company company)
        {
            return _optionsAggregator.GetCompanyYears(company);
        }

        public (bool success, string message) SaveWebDocumentToWebShare(IDocument doc)
        {
            return _docBroker.SaveWebDocumentToWebShare(doc);
        }

        public ISearchExpressionGroup CreateSearchExpression(SPDocumentType documentType, SPFieldNames enumValue, CamlComparison comparison, int? value)
        {
            return new SearchExpressionGroup(_documentFactory.CreateDocument(documentType), enumValue, comparison, value);
        }

        public ISearchExpressionGroup CreateSearchExpression(SPDocumentType documentType, SPFieldNames enumValue, CamlComparison comparison, long? value)
        {
            return new SearchExpressionGroup(_documentFactory.CreateDocument(documentType), enumValue, comparison, value);
        }

        public ISearchExpressionGroup CreateSearchExpression(SPDocumentType documentType, SPFieldNames enumValue, CamlComparison comparison, string value)
        {
            return new SearchExpressionGroup(_documentFactory.CreateDocument(documentType), enumValue, comparison, value);
        }

        public ISearchExpressionGroup CreateSearchExpression(SPDocumentType documentType,
                                                             SPFieldNames enumValue,
                                                             CamlComparison comparison,
                                                             DateTime? value)
        {
            return new SearchExpressionGroup(_documentFactory.CreateDocument(documentType), enumValue, comparison, value);
        }

        public void Dispose()
        {
            _docBroker?.Dispose();
        }
    }
}
