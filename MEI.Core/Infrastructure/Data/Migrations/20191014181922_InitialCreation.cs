using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MEI.Core.Infrastructure.Data.Migrations
{
    public partial class InitialCreation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Travel");

            migrationBuilder.CreateTable(
                name: "AuditEntry",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    WhenExecuted = table.Column<DateTimeOffset>(nullable: false),
                    UserName = table.Column<string>(nullable: true),
                    Operation = table.Column<string>(nullable: true),
                    Data = table.Column<string>(nullable: true),
                    AppName = table.Column<string>(nullable: true),
                    MachineName = table.Column<string>(nullable: true),
                    Environment = table.Column<string>(nullable: true),
                    CorrelationId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditEntry", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Client",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    StreetAddressLine1 = table.Column<string>(nullable: true),
                    StreetAddressLine2 = table.Column<string>(nullable: true),
                    CityTown = table.Column<string>(nullable: true),
                    StateProvince = table.Column<string>(nullable: true),
                    PostalCode = table.Column<string>(nullable: true),
                    Country = table.Column<string>(nullable: true),
                    WebsiteUrl = table.Column<string>(nullable: true),
                    TollFreePhoneNumber = table.Column<string>(nullable: true),
                    Initials = table.Column<string>(nullable: true),
                    WhenCreated = table.Column<DateTimeOffset>(nullable: false),
                    WhenDeactivated = table.Column<DateTimeOffset>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Client", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Country",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    IsoCode = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Country", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Currency",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    IsoSymbol = table.Column<string>(nullable: true),
                    Symbol = table.Column<string>(nullable: true),
                    RegionName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currency", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Log",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    MachineName = table.Column<string>(maxLength: 200, nullable: true),
                    WhenLogged = table.Column<DateTimeOffset>(nullable: false),
                    Level = table.Column<string>(maxLength: 5, nullable: true),
                    Message = table.Column<string>(nullable: true),
                    Logger = table.Column<string>(maxLength: 300, nullable: true),
                    Properties = table.Column<string>(nullable: true),
                    Callsite = table.Column<string>(maxLength: 300, nullable: true),
                    Exception = table.Column<string>(nullable: true),
                    Environment = table.Column<string>(maxLength: 200, nullable: true),
                    AppName = table.Column<string>(maxLength: 500, nullable: true),
                    CorrelationId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Log", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowCategory",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    Description = table.Column<string>(maxLength: 250, nullable: true),
                    WhenInactivated = table.Column<DateTimeOffset>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowCategory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Invoice",
                schema: "Travel",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(nullable: true),
                    SabreInvoiceId = table.Column<string>(nullable: true),
                    ProgramId = table.Column<int>(nullable: true),
                    ProgramName = table.Column<string>(nullable: true),
                    SpeakerId = table.Column<int>(nullable: true),
                    SpeakerName = table.Column<string>(nullable: true),
                    ClientId = table.Column<int>(nullable: true),
                    SubmittingUser = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoice", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Invoice_Client_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Client",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StateProvince",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Abbreviation = table.Column<string>(nullable: true),
                    CountryId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StateProvince", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StateProvince_Country_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Country",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Service",
                schema: "Travel",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    FeeAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FeeCurrencyId = table.Column<int>(nullable: false)
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

            migrationBuilder.CreateTable(
                name: "WorkflowStep",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    WorkflowCategoryId = table.Column<int>(nullable: false),
                    StepOrder = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    Description = table.Column<string>(maxLength: 250, nullable: true),
                    WhenInactivated = table.Column<DateTimeOffset>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowStep", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkflowStep_WorkflowCategory_WorkflowCategoryId",
                        column: x => x.WorkflowCategoryId,
                        principalTable: "WorkflowCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvoiceLineItem",
                schema: "Travel",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Quantity = table.Column<long>(nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    InvoiceId = table.Column<int>(nullable: false),
                    ServiceId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceLineItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvoiceLineItem_Invoice_InvoiceId",
                        column: x => x.InvoiceId,
                        principalSchema: "Travel",
                        principalTable: "Invoice",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvoiceLineItem_Service_ServiceId",
                        column: x => x.ServiceId,
                        principalSchema: "Travel",
                        principalTable: "Service",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvoiceWorkflowStatus",
                schema: "Travel",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    WorkflowStepId = table.Column<int>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: false),
                    WhenCreated = table.Column<DateTimeOffset>(nullable: false),
                    Notes = table.Column<string>(nullable: true),
                    InvoiceId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceWorkflowStatus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvoiceWorkflowStatus_Invoice_InvoiceId",
                        column: x => x.InvoiceId,
                        principalSchema: "Travel",
                        principalTable: "Invoice",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvoiceWorkflowStatus_WorkflowStep_WorkflowStepId",
                        column: x => x.WorkflowStepId,
                        principalTable: "WorkflowStep",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StateProvince_CountryId",
                table: "StateProvince",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowStep_WorkflowCategoryId",
                table: "WorkflowStep",
                column: "WorkflowCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_ClientId",
                schema: "Travel",
                table: "Invoice",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceLineItem_InvoiceId",
                schema: "Travel",
                table: "InvoiceLineItem",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceLineItem_ServiceId",
                schema: "Travel",
                table: "InvoiceLineItem",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceWorkflowStatus_InvoiceId",
                schema: "Travel",
                table: "InvoiceWorkflowStatus",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceWorkflowStatus_WorkflowStepId",
                schema: "Travel",
                table: "InvoiceWorkflowStatus",
                column: "WorkflowStepId");

            migrationBuilder.CreateIndex(
                name: "IX_Service_FeeCurrencyId",
                schema: "Travel",
                table: "Service",
                column: "FeeCurrencyId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditEntry");

            migrationBuilder.DropTable(
                name: "Log");

            migrationBuilder.DropTable(
                name: "StateProvince");

            migrationBuilder.DropTable(
                name: "InvoiceLineItem",
                schema: "Travel");

            migrationBuilder.DropTable(
                name: "InvoiceWorkflowStatus",
                schema: "Travel");

            migrationBuilder.DropTable(
                name: "Country");

            migrationBuilder.DropTable(
                name: "Service",
                schema: "Travel");

            migrationBuilder.DropTable(
                name: "Invoice",
                schema: "Travel");

            migrationBuilder.DropTable(
                name: "WorkflowStep");

            migrationBuilder.DropTable(
                name: "Currency");

            migrationBuilder.DropTable(
                name: "Client");

            migrationBuilder.DropTable(
                name: "WorkflowCategory");
        }
    }
}
