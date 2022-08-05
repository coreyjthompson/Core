using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using MEI.Core.Commands;
using MEI.Core.Commands.Decorators;
using MEI.Core.Infrastructure.Commands.Decorators;
using MEI.Core.Infrastructure.Ldap.Queries;
using MEI.Core.Infrastructure.Queries;
using MEI.Core.Infrastructure.Queries.Decorators;
using MEI.Core.Queries;
using MEI.Core.Validation;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MEI.Core.Infrastructure.Helpers
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCoreInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IQueryProcessor, DynamicQueryProcessor>();

            // Example of using Decorate and explicitly calling a constructor in case of more needed dependencies
            /*services.Decorate<ICommandHandler<AddInvoiceCommand>>((inner, provider) =>
                new AddInvoiceCommandHandler(provider.GetService<CoreContext>()));*/

            services.AddScoped<ICommandProcessor, DynamicCommandProcessor>();

            var postCommitImpl = new PostCommitRegistrator();

            services.AddScoped(provider => postCommitImpl)
                .AddScoped<IPostCommitRegistrator>(provider => postCommitImpl);

            services.AddScoped<IValidator, DataAnnotationValidator>();

            services.AddLazyCache();

            services.AddTransient<IStopwatch, SystemStopwatch>();

            // Use the .AddQueryHandlers extension method to add all queries within a given assembly
            // and include any decorators that should be applied to them all
            services.AddQueryHandlers(typeof(FindByIdentityQuery),
                new[]
                {
                    typeof(RunTimeLogQueryHandlerDecorator<,>),
                    typeof(ValidationQueryHandlerDecorator<,>),
                    typeof(CacheQueryHandlerDecorator<,>)
                });

            // Or add the query individually if they should be different than the rest

            //// Use the .AddCommandHandlers extension method to add all commands within a given assembly
            //// and include any decorators that should be applied to them all
            //services.AddCommandHandlers(typeof(Demo_AddInvoiceCommand),
            //    new[]
            //    {
            //        typeof(RunTimeLogCommandHandlerDecorator<,>),
            //        typeof(AuditingCommandHandlerDecorator<,>),
            //        typeof(TransactionCommandHandlerDecorator<,>),
            //        typeof(PostCommitCommandHandlerDecorator<,>)
            //    });

            // Or add the command individually if they should be different than the rest
            /*services.AddTransient<ICommandHandler<AddInvoiceCommand>, AddInvoiceCommandHandler>();
            services.Decorate<ICommandHandler<AddInvoiceCommand>, TransactionCommandHandlerDecorator<AddInvoiceCommand>>();
            services.Decorate<ICommandHandler<AddInvoiceCommand>, PostCommitCommandHandlerDecorator<AddInvoiceCommand>>();*/

            services.Configure<InfrastructureOptions>(configuration.GetSection("InfrastructureOptions"));

            return services;
        }

        public static IServiceCollection AddQueryHandlers(this IServiceCollection services, Type[] decorateWith = null)
        {
            services.AddQueryHandlers(typeof(IQueryHandler<,>), decorateWith);

            return services;
        }

        public static IServiceCollection AddQueryHandlers(this IServiceCollection services, Type referenceType, Type[] decorateWith)
        {
            return AddHandlers(services, Assembly.GetAssembly(referenceType).GetTypes(), typeof(IQuery<>), typeof(IQueryHandler<,>), decorateWith);
        }

        public static IServiceCollection AddCommandHandlers(this IServiceCollection services, Type[] decorateWith = null)
        {
            services.AddCommandHandlers(typeof(ICommand<>), decorateWith);

            return services;
        }

        public static IServiceCollection AddCommandHandlers(this IServiceCollection services, Type referenceType, Type[] decorateWith)
        {
            return services.AddHandlers(Assembly.GetAssembly(referenceType).GetTypes(), typeof(ICommand<>), typeof(ICommandHandler<,>), decorateWith);
        }

        private static IServiceCollection AddHandlers(this IServiceCollection services, Type[] typesToSearch, Type baseInterfaceType, Type handlerInterfaceType, Type[] decorateWith)
        {
            var types = GetBaseAndResultTypes(typesToSearch, baseInterfaceType);

            foreach ((Type baseType, Type resultType) in types)
            {
                Type handlerType = GetHandlerType(typesToSearch, baseType);

                Type serviceType = handlerInterfaceType.MakeGenericType(baseType, resultType);

                services.AddTransient(serviceType, handlerType);

                if (decorateWith == null)
                {
                    continue;
                }

                foreach (Type t in decorateWith)
                {
                    services.Decorate(serviceType, t.MakeGenericType(baseType, resultType));
                }
            }

            return services;
        }

        private static IEnumerable<(Type baseType, Type resultType)> GetBaseAndResultTypes(IEnumerable<Type> typesToSearch, Type interfaceType)
        {
            return (from x in typesToSearch
                    from z in x.GetInterfaces()
                    let y = x.BaseType
                    where (y != null && y.IsGenericType && interfaceType.IsAssignableFrom(y.GetGenericTypeDefinition()))
                          || (z != null && z.IsGenericType && interfaceType.IsAssignableFrom(z.GetGenericTypeDefinition()))
                    select (baseType: x, resultType: z.GenericTypeArguments[0])).ToArray();
        }

        private static Type GetHandlerType(IEnumerable<Type> typesToSearch, Type baseType)
        {
            return (from x in typesToSearch
                    from z in x.GetInterfaces()
                    let y = x.BaseType
                    where (y != null && y.IsGenericType && y.GenericTypeArguments.Length > 0
                           && baseType.IsAssignableFrom(y.GenericTypeArguments[0]))
                          || (z != null && z.IsGenericType && z.GenericTypeArguments.Length > 0
                              && baseType.IsAssignableFrom(z.GenericTypeArguments[0]))
                    select x).FirstOrDefault();
        }
    }
}