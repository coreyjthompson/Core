using MEI.Core.Commands.Decorators;
using MEI.Core.Infrastructure.Commands.Decorators;
using MEI.Core.Infrastructure.Helpers;
using MEI.Core.Infrastructure.Queries.Decorators;
using MEI.SPDocuments;
using MEI.SPDocuments.Helpers;
using MEI.Travel.Commands;
using MEI.Travel.Queries;
using MEI.Travel.Services;

using Microsoft.Extensions.DependencyInjection;

namespace MEI.Travel.Helpers
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTravelServices(this IServiceCollection services, string startDirectory)
        {
            services.AddQueryHandlers(
                typeof(GetInvoiceByIdQuery),
                new[] {typeof(RunTimeLogQueryHandlerDecorator<,>), typeof(ValidationQueryHandlerDecorator<,>), typeof(CacheQueryHandlerDecorator<,>)}
            );

            services.AddCommandHandlers(
                typeof(AddInvoiceCommand),
                new[] {/*typeof(TransactionCommandHandlerDecorator<,>),*/ typeof(RunTimeLogCommandHandlerDecorator<,>), typeof(AuditingCommandHandlerDecorator<,>)}
            );

            // Add sharepoint documents 
            services.AddSPDocuments(startDirectory);

            // Add invoice service
            services.AddScoped<IInvoiceService, InvoiceService>();

            return services;
        }
    }
}