using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security;

using MEI.SPDocuments.TypeCodes;

using Microsoft.Extensions.Options;

namespace MEI.SPDocuments
{
    public class SPDocumentsOptions
    {
        public IList<string> LdapPaths { get; set; }

        public string CredentialUserName { get; set; }

        public string CredentialPassword { get; set; }

        public SecureString SecureCredentialPassword => new NetworkCredential("", CredentialPassword).SecurePassword;

        public string CredentialDomain { get; set; }

        public string BaseDocumentPath { get; set; }

        public string BaseWebPath { get; set; }

        public string SignInUploadTimeoutAlert { get; set; }

        public string SmtpServer { get; set; }

        public IList<SPDocumentsOptionsCompany> Companies { get; set; }

        public SPDocumentsOptionsConnectionStrings ConnectionStrings { get; set; }

        public string GetCompanyByUrl(string url)
        {
            return Companies.FirstOrDefault(x => x.SiteRelativeUrl == url)?.Name;
        }

        public string BaseSiteUrl { get; set; }

        public IDictionary<string, string> DocumentLibraryTitles { get; set; }
    }

    public interface ISPDocumentsOptionsAggregator
    {
        string GetCompanyConnectionString(Company company, DocumentYear year);

        string GetDocumentLibraryTitle(Company company, SPDocumentType documentType);

        string GetSiteRelativeUrl(Company company);

        SortedList<string, string> GetCompanyYears(Company company);

        bool IsEnabled(Company company, SPDocumentType documentType);
    }

    public class SPDocumentsOptionsAggregator
        : ISPDocumentsOptionsAggregator
    {
        private readonly SPDocumentsOptions _options;
        private readonly IDocumentInfoAggregator _documentInfoAggregator;

        public SPDocumentsOptionsAggregator(IOptions<SPDocumentsOptions> options, IDocumentInfoAggregator documentInfoAggregator)
        {
            _options = options.Value;
            _documentInfoAggregator = documentInfoAggregator;
        }

        public string GetCompanyConnectionString(Company company, DocumentYear year)
        {
            Preconditions.CheckEnum("company", company, Company.Undefined);
            Preconditions.CheckEnum("year", year, DocumentYear.Undefined);

            string companyName = company.ToDisplayNameLong();
            string yearName = year.ToDisplayNameLong();

            IList<SPDocumentsOptionsCompanyConnectionString> connectionStrings = _options.Companies.FirstOrDefault(x => x.Name == companyName)?.ConnectionStrings;

            if (connectionStrings == null)
            {
                throw new ArgumentException(string.Format("Unable to find connection string. {0} {1}", company, year));
            }

            return connectionStrings.FirstOrDefault(y => y.Name == yearName)?.Value;
        }

        public string GetDocumentLibraryTitle(Company company, SPDocumentType documentType)
        {
            Preconditions.CheckEnum(nameof(company), company, Company.Undefined);
            Preconditions.CheckEnum(nameof(documentType), documentType, SPDocumentType.None);

            string companyName = company.ToDisplayNameLong();
            string acronym = _documentInfoAggregator.CodeToAcronym(documentType);

            string overriddenDocumentTitle = _options.Companies.FirstOrDefault(x => x.Name == companyName)?.Documents
                ?.FirstOrDefault(y => y.Acronym == acronym)?.DocumentLibraryTitle;

            if (!string.IsNullOrEmpty(overriddenDocumentTitle))
            {
                return overriddenDocumentTitle;
            }

            if (!_options.DocumentLibraryTitles.ContainsKey(acronym))
            {
                throw new Exception("Document type is missing a default document library title. " + documentType);
            }

            return _options.DocumentLibraryTitles[acronym];
        }

        public string GetSiteRelativeUrl(Company company)
        {
            Preconditions.CheckEnum(nameof(company), company, Company.Undefined);

            string companyName = company.ToDisplayNameLong();

            string siteRelativeUrl = _options.Companies.FirstOrDefault(x => x.Name == companyName)?.SiteRelativeUrl;

            if (string.IsNullOrWhiteSpace(siteRelativeUrl))
            {
                throw new Exception("SiteRelativeUrl is missing and is required. " + company);
            }

            return siteRelativeUrl;
        }

        public SortedList<string, string> GetCompanyYears(Company company)
        {
            Preconditions.CheckEnum("company", company, Company.Undefined);

            string companyName = company.ToDisplayNameLong();

            var connectionStrings = _options.Companies
                .FirstOrDefault(x => x.Name == companyName)?.ConnectionStrings;

            if (connectionStrings == null)
            {
                throw new ArgumentException(string.Format("Unable to find connection strings. {0}", company));
            }

            Dictionary<string, string> items = connectionStrings
                .Where(x => x.Name.ToDocumentYear() != DocumentYear.Undefined)
                .OrderBy(x => x.Name)
                .ToDictionary(q => q.Name, q => q.Name.ToDocumentYear().ToDisplayNameLong());

            return new SortedList<string, string>(items);
        }

        public bool IsEnabled(Company company, SPDocumentType documentType)
        {
            Preconditions.CheckEnum("company", company, Company.Undefined);
            Preconditions.CheckEnum("documentType", documentType, SPDocumentType.None);

            string companyName = company.ToDisplayNameLong();
            string acronym = _documentInfoAggregator.CodeToAcronym(documentType);

            SPDocumentsOptionsDocument document = _options.Companies.FirstOrDefault(x => x.Name == companyName)?.Documents
                ?.FirstOrDefault(y => y.Acronym == acronym);

            // document is assumed to be enabled unless explicitly marked disabled
            if (document == null)
            {
                return true;
            }

            return !document.Disabled;
        }
    }

    public class SPDocumentsOptionsCompany
    {
        public string Name { get; set; }

        public IList<SPDocumentsOptionsCompanyConnectionString> ConnectionStrings { get; set; }

        public string SiteRelativeUrl { get; set; }

        public IList<SPDocumentsOptionsDocument> Documents { get; set; }

        public override string ToString()
        {
            return string.Format("[Name={0}]", Name);
        }
    }

    public class SPDocumentsOptionsDocument
    {
        public string Acronym { get; set; }

        public string DocumentLibraryTitle { get; set; }

        public bool Disabled { get; set; }
    }

    public class SPDocumentsOptionsConnectionStrings
    {
        public string Event { get; set; }
    }

    public class SPDocumentsOptionsCompanyConnectionString
    {
        public string Name { get; set; }

        public string Value { get; set; }

        public override string ToString()
        {
            return string.Format("[Name={0}, Value={1}]", Name, Value);
        }
    }
}
