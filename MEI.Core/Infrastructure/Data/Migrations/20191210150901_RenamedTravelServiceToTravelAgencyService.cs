using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MEI.Core.Infrastructure.Data.Migrations
{
    public partial class RenamedTravelServiceToTravelAgencyService : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceLineItem_Service_ServiceId",
                schema: "Travel",
                table: "InvoiceLineItem");

            migrationBuilder.DropTable(
                name: "Service",
                schema: "Travel");

            migrationBuilder.CreateTable(
                name: "AgencyService",
                schema: "Travel",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    FeeAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FeeCurrencyId = table.Column<int>(nullable: false),
                    SortOrder = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgencyService", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AgencyService_Currency_FeeCurrencyId",
                        column: x => x.FeeCurrencyId,
                        principalTable: "Currency",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AgencyService_FeeCurrencyId",
                schema: "Travel",
                table: "AgencyService",
                column: "FeeCurrencyId");

            migrationBuilder.RenameColumn(
                name: "ServiceId",
                schema: "Travel",
                table: "InvoiceLineItem",
                newName: "AgencyServiceId");

            migrationBuilder.RenameIndex(
                name: "IX_InvoiceLineItem_ServiceId",
                schema: "Travel",
                table: "InvoiceLineItem",
                newName: "IX_InvoiceLineItem_AgencyServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceLineItem_AgencyService_AgencyServiceId",
                schema: "Travel",
                table: "InvoiceLineItem",
                column: "AgencyServiceId",
                principalSchema: "Travel",
                principalTable: "AgencyService",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceLineItem_AgencyService_ServiceId",
                schema: "Travel",
                table: "InvoiceLineItem");

            migrationBuilder.DropTable(
                name: "AgencyService",
                schema: "Travel");

            migrationBuilder.CreateTable(
                name: "Service",
                schema: "Travel",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FeeAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FeeCurrencyId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    SortOrder = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Service", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Service_Currency_FeeCurrencyId",
                        column: x => x.FeeCurrencyId,
                        principalTable: "Currency",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Service_FeeCurrencyId",
                schema: "Travel",
                table: "Service",
                column: "FeeCurrencyId");

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceLineItem_Service_ServiceId",
                schema: "Travel",
                table: "InvoiceLineItem",
                column: "ServiceId",
                principalSchema: "Travel",
                principalTable: "Service",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceLineItem_AgencyService_AgencyServiceId",
                schema: "Travel",
                table: "InvoiceLineItem");

            migrationBuilder.RenameColumn(
                name: "AgencyServiceId",
                schema: "Travel",
                table: "InvoiceLineItem",
                newName: "ServiceId");

            migrationBuilder.RenameIndex(
                name: "IX_InvoiceLineItem_AgencyServiceId",
                schema: "Travel",
                table: "InvoiceLineItem",
                newName: "IX_InvoiceLineItem_ServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceLineItem_AgencyService_ServiceId",
                schema: "Travel",
                table: "InvoiceLineItem",
                column: "ServiceId",
                principalSchema: "Travel",
                principalTable: "AgencyService",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

        }
    }
}
