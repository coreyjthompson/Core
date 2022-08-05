using Microsoft.EntityFrameworkCore.Migrations;

namespace MEI.Core.Infrastructure.Data.Migrations
{
    public partial class RemovedSubmittingUSerPropertyFromInvoiceModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubmittingUser",
                schema: "Travel",
                table: "Invoice");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SubmittingUser",
                schema: "Travel",
                table: "Invoice",
                nullable: true);
        }
    }
}
