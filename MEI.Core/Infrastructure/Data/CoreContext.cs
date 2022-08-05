using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using MEI.Core.DomainModels;
using MEI.Core.DomainModels.Travel.Aggregates;
using MEI.Core.Infrastructure.Services;
using MEI.Logging;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

namespace MEI.Core.Infrastructure.Data
{
    public class CoreContext
        : DbContext
    {
        private readonly ICorrelationProvider _correlationProvider;
        private readonly IUserResolverService _userResolverService;

        public CoreContext(DbContextOptions<CoreContext> options, IUserResolverService userResolverService, ICorrelationProvider correlationProvider)
            : base(options)
        {
            _userResolverService = userResolverService;
            _correlationProvider = correlationProvider;
        }

        public virtual DbSet<DomainModels.Travel.Invoice> TravelInvoices { get; set; }

        public virtual DbSet<DomainModels.Common.Log> Logs { get; set; }

        public virtual DbSet<DomainModels.Common.Client> Clients { get; set; }

        public virtual DbSet<DomainModels.Travel.InvoiceLineItem> TravelInvoiceLineItems { get; set; }

        public virtual DbSet<DomainModels.Travel.InvoiceWorkflowStatus> TravelInvoiceWorkflowStatuses { get; set; }

        public virtual DbSet<DomainModels.Travel.AgencyService> AgencyServices { get; set; }

        public virtual DbSet<DomainModels.Common.WorkflowCategory> WorkflowCategories { get; set; }

        public virtual DbSet<DomainModels.Common.WorkflowStep> WorkflowSteps { get; set; }

        public virtual DbSet<DomainModels.Common.AuditEntry> AuditEntries { get; set; }

        public virtual DbSet<DomainModels.Common.Currency> Currencies { get; set; }

        public virtual DbSet<DomainModels.Common.Country> Countries { get; set; }

        public virtual DbSet<DomainModels.Common.StateProvince> StateProvinces { get; set; }

        public virtual DbQuery<DomainModels.Travel.Aggregates.InvoiceHistoryLine> InvoiceHistoryLines { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            void AssignSchemaAndRemoveNamespaceFromName(IMutableEntityType entityType, string tableName, string schemaName)
            {
                entityType.Relational().Schema = schemaName;

                entityType.Relational().TableName =
                    tableName?.StartsWith(schemaName) == true ? tableName.Substring(schemaName.Length) : tableName;
            }

            modelBuilder.Query<InvoiceHistoryLine>().ToView("vw_InvoiceTemporalHistory");

            //Debugger.Launch();

            // Set SQL default values and Computed SQL values on all fields
            // that inherit from a given type

            /*foreach (var entityType in modelBuilder.Model.GetEntityTypes()
                .Where(e => typeof(AuditModelBase).IsAssignableFrom(e.ClrType)))
            {
                modelBuilder
                    .Entity(entityType.ClrType)
                    .Property("CreatedBy")
                    .HasDefaultValueSql("suser_name()");

                modelBuilder
                    .Entity(entityType.ClrType)
                    .Property("LoggedInUser")
                    .HasComputedColumnSql("suser_name()");
            }*/

            // Make table names singular
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var singularName = entityType.Relational().TableName.Singularize();

                var travelNamespace = typeof(DomainModels.Travel.Invoice)?.Namespace;

                if (!string.IsNullOrEmpty(travelNamespace) && entityType.ClrType.Namespace?.StartsWith(travelNamespace) == true)
                {
                    AssignSchemaAndRemoveNamespaceFromName(entityType, singularName, "Travel");
                }
                else
                {
                    entityType.Relational().TableName = singularName;
                }
            }

            base.OnModelCreating(modelBuilder);
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            AddAuditInfo();

            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override int SaveChanges()
        {
            AddAuditInfo();

            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new CancellationToken())
        {
            AddAuditInfo();

            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            AddAuditInfo();

            return base.SaveChangesAsync(cancellationToken);
        }

        private void AddAuditInfo()
        {
            // Set ModifiedBy field to current username
            /*var entries = ChangeTracker.Entries().Where(x =>
                x.Entity is DomainModels.AuditModelBase && (x.State == EntityState.Added || x.State == EntityState.Modified));

            string userName = _userResolverService.GetUserName();

            foreach (EntityEntry entityEntry in entries)
            {
                if (!(entityEntry.Entity is DomainModels.AuditModelBase baseEntity))
                {
                    continue;
                }

                baseEntity.ModifiedBy = userName;
            }*/

            var transactionIdEntries = ChangeTracker.Entries().Where(x => x.Entity is ICorrelationId);

            foreach (var transactionIdEntry in transactionIdEntries)
            {
                if (!(transactionIdEntry.Entity is ICorrelationId baseEntity))
                {
                    continue;
                }

                baseEntity.CorrelationId = _correlationProvider.GetCorrelationId();
            }
        }
    }

    public class CoreContextDesignTimeFactory
        : IDesignTimeDbContextFactory<CoreContext>
    {
        public CoreContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var builder = new DbContextOptionsBuilder<CoreContext>();
            builder.UseSqlServer(configuration["ConnectionStrings:Core"]);

            return new CoreContext(builder.Options, new DummyUserResolverService(), new DummyCorrelationProvider());
        }

        private class DummyUserResolverService
            : IUserResolverService
        {
            public string GetUserName()
            {
                return "<DesignTime>";
            }
        }

        private class DummyCorrelationProvider
            : ICorrelationProvider
        {
            public string GetCorrelationId()
            {
                return "<DesignTime>";
            }
        }
    }
}