using MEI.Core.Infrastructure.Data.Helpers;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MEI.Core.Infrastructure.Data.Migrations
{
    public partial class AddInitialTemporalTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema("History");
            migrationBuilder.AddTemporalTableSupport("Travel", "Invoice", "History");
            migrationBuilder.AddTemporalTableSupport("Travel", "InvoiceLineItem", "History");
            migrationBuilder.AddTemporalTableSupport("Travel", "AgencyService", "History");
            migrationBuilder.AddTemporalTableSupport("dbo", "Client", "History");
            migrationBuilder.AddTemporalTableSupport("dbo", "WorkflowCategory", "History");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
