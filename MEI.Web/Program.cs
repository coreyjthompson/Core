using System;

using MEI.Core.Infrastructure.Data;
using MEI.Logging;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MEI.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IHost host = CreateHostBuilder(args).Build();
            var logger = host.Services.GetRequiredService<ILogger<Program>>();
            var logManager = host.Services.GetService<ILogManager>();

            logManager.ConfigureVariables();

            try
            {
                ApplicationOptions options = host.Services.GetRequiredService<IOptions<ApplicationOptions>>().Value;

                logger.LogDebug("init main - " + options.AppName);

                if (options.SeedData)
                {
                    using IServiceScope serviceScope = host.Services.CreateScope();
                    using var context = serviceScope.ServiceProvider.GetService<CoreContext>();

                    context.Seed();
                }

                host.Run();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Stopped program because of exception.");

                logManager?.Shutdown();

                throw;
            }
            finally
            {
                logManager?.Shutdown();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                    .UseStartup<Startup>()
                    .UseSetting(WebHostDefaults.DetailedErrorsKey, "true");
                });
    }
}
