using System.IO;

using MEI.SPDocuments.ActiveDirectory;
using MEI.SPDocuments.Data;
using MEI.SPDocuments.Security;
using MEI.SPDocuments.Services;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using NodaTime;

namespace MEI.SPDocuments.Helpers
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSPDocuments(this IServiceCollection services)
        {
            return services.AddSPDocuments(Directory.GetCurrentDirectory());
        }

        public static IServiceCollection AddSPDocuments(this IServiceCollection services, string spDocumentsSettingsPath)
        {
            var configuration = new ConfigurationBuilder().SetBasePath(spDocumentsSettingsPath)
                .AddJsonFile("spDocumentsSettings.json", false, true).Build();

            services.AddLogging()
                .AddOptions()
                .Configure<SPDocumentsOptions>(configuration);

            services
                .AddScoped<IDbUtilities, DbUtilities>()
                .AddSingleton<ICacheProvider, DefaultCacheProvider>()
                .AddScoped<IRepository, CachedRepository>()
                .AddScoped<IDocumentInfoAggregator, DocumentInfoAggregator>()
                .AddScoped<IEmailer, Emailer>()
                .AddScoped<IDocumentFactory, DocumentFactory>()
                .AddSingleton<IClock>(SystemClock.Instance)
                .AddScoped<IEncryptor, TripleDESEncryptor>()
                .AddScoped<IServiceManager, SPServiceManager>()
                .AddScoped<IActiveDirectoryControl, ActiveDirectoryControl>()
                .AddScoped<IDocumentAccessControl, DocumentAccessControl>()
                .AddScoped<IPdfTools, PdfTools>()
                .AddScoped<ISPDocumentsOptionsAggregator, SPDocumentsOptionsAggregator>()
                .AddScoped<IFileDownloader, WebDownloader>()
                .AddScoped<IDocumentBroker, DocumentBroker>()
                .AddScoped<ServiceCache>()
                .AddScoped<ISPDocuments, SP_Documents>()
                .AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            return services;
        }
    }
}