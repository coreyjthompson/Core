using System;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MEI.Core.Infrastructure.Data.Helpers
{
    public static class CoreDataServiceCollectionExtensions
    {
        public static IServiceCollection AddCoreData(this IServiceCollection services, IConfiguration config)
        {
            if (config["ConnectionStrings:Core"] == null)
            {
                throw new Exception("Core database connection string not found at: ConnectionStrings:Core");
            }

            return services
                .AddEntityFrameworkSqlServer()
                .AddDbContext<CoreContext>(options => options.UseSqlServer(config["ConnectionStrings:Core"]));
        }
    }
}