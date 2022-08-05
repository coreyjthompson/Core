using System;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Storage;

namespace MEI.Core.Infrastructure.Data.Helpers
{
    public static class IQueryableExtensions
    {
        private static readonly TypeInfo QueryCompilerTypeInfo = typeof(QueryCompiler).GetTypeInfo();

        private static readonly FieldInfo QueryCompilerField =
            typeof(EntityQueryProvider).GetTypeInfo().DeclaredFields.First(x => x.Name == "_queryCompiler");

        private static readonly FieldInfo QueryModelGeneratorField =
            QueryCompilerTypeInfo.DeclaredFields.First(x => x.Name == "_queryModelGenerator");

        private static readonly FieldInfo DatabaseField = QueryCompilerTypeInfo.DeclaredFields.Single(x => x.Name == "_database");

        private static readonly PropertyInfo DatabaseDependenciesField =
            typeof(Database).GetTypeInfo().DeclaredProperties.Single(x => x.Name == "Dependencies");

        public static string ToSql<TEntity>(this IQueryable<TEntity> query)
            where TEntity : class
        {
            var queryCompiler = (QueryCompiler) QueryCompilerField.GetValue(query.Provider);
            var modelGenerator = (QueryModelGenerator) QueryModelGeneratorField.GetValue(queryCompiler);
            var queryModel = modelGenerator.ParseQuery(query.Expression);
            var database = (IDatabase) DatabaseField.GetValue(queryCompiler);
            var databaseDependencies = (DatabaseDependencies) DatabaseDependenciesField.GetValue(database);
            var queryCompilationContext = databaseDependencies.QueryCompilationContextFactory.Create(false);
            var modelVisitor = (RelationalQueryModelVisitor) queryCompilationContext.CreateQueryModelVisitor();
            modelVisitor.CreateQueryExecutor<TEntity>(queryModel);

            return modelVisitor.Queries.First().ToString();
        }

        private static DbContext GetDbContext<T>(this DbQuery<T> dbQuery) where T : class
        {
            var infrastructure = dbQuery as IInfrastructure<IServiceProvider>;
            return (infrastructure.Instance.GetService(typeof(ICurrentDbContext)) as ICurrentDbContext).Context;
        }

        private static DbContext GetDbContext<T>(this DbSet<T> dbSet) where T : class
        {
            var infrastructure = dbSet as IInfrastructure<IServiceProvider>;
            return (infrastructure.Instance.GetService(typeof(ICurrentDbContext)) as ICurrentDbContext).Context;
        }

    }
}