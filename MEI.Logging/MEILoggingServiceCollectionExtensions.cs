using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using NLog.Extensions.Logging;

using MEI.Logging;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class MEILoggingServiceCollectionExtensions
    {
        public static IServiceCollection AddMEILogging(this IServiceCollection services, IConfiguration config)
        {
            return services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                loggingBuilder.AddNLog(config);
            }).AddSingleton<ILogManager, NLogManager>();
        }
    }
}
