using System;
using System.Collections.Generic;

using MEI.SPDocuments.TypeCodes;
using Microsoft.Extensions.Options;

namespace MEI.SPDocuments.Services
{
    internal class ServiceCache
        : Dictionary<Company, DocumentContext>, IDisposable
    {
        private static readonly object LockObject = new object();
        private readonly SPDocumentsOptions _options;
        private readonly ISPDocumentsOptionsAggregator _optionsAggregator;

        public ServiceCache(IOptions<SPDocumentsOptions> options, ISPDocumentsOptionsAggregator optionsAggregator)
        {
            _options = options.Value;
            _optionsAggregator = optionsAggregator;
        }

        public new DocumentContext this[Company company]
        {
            get
            {
                Preconditions.CheckEnum("company", company, Company.Undefined);

                lock (LockObject)
                {
                    (Company company, DocumentContext serviceType)? kv = GetItemByCompany(company);
                    if (!kv.HasValue)
                    {
                        DocumentContext s = new DocumentContext(_options.BaseSiteUrl + _optionsAggregator.GetSiteRelativeUrl(company),
                            _options.CredentialUserName + "@meintl.com",
                            _options.SecureCredentialPassword);

                        Add(company, s);

                        return s;
                    }

                    return kv.Value.serviceType;
                }
            }
        }

        private (Company company, DocumentContext serviceType)? GetItemByCompany(Company company)
        {
            foreach (KeyValuePair<Company, DocumentContext> kv in this)
            {
                if (kv.Key != company)
                {
                    continue;
                }

                return (kv.Key, kv.Value);
            }

            return null;
        }

        public void Dispose()
        {
            foreach (KeyValuePair<Company, DocumentContext> kv in this)
            {
                (kv.Value as IDisposable)?.Dispose();
            }
        }
    }
}
