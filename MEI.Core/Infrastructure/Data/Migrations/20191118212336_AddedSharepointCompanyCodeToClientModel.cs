using Microsoft.EntityFrameworkCore.Migrations;

namespace MEI.Core.Infrastructure.Data.Migrations
{
    public partial class AddedSharepointCompanyCodeToClientModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                schema: "Travel",
                table: "Invoice");

            migrationBuilder.RenameColumn(
                name: "SpeakerName",
                schema: "Travel",
                table: "Invoice",
                newName: "EventName");

            migrationBuilder.RenameColumn(
                name: "SpeakerId",
                schema: "Travel",
                table: "Invoice",
                newName: "EventId");

            migrationBuilder.RenameColumn(
                name: "ProgramName",
                schema: "Travel",
                table: "Invoice",
                newName: "ConsultantName");

            migrationBuilder.RenameColumn(
                name: "ProgramId",
                schema: "Travel",
                table: "Invoice",
                newName: "ConsultantId");

            migrationBuilder.AddColumn<string>(
                name: "SharePointCompanyCode",
                table: "Client",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SharePointCompanyCode",
                table: "Client");

            migrationBuilder.RenameColumn(
                name: "EventName",
                schema: "Travel",
                table: "Invoice",
                newName: "SpeakerName");

            migrationBuilder.RenameColumn(
                name: "EventId",
                schema: "Travel",
                table: "Invoice",
                newName: "SpeakerId");

            migrationBuilder.RenameColumn(
                name: "ConsultantName",
                schema: "Travel",
                table: "Invoice",
                newName: "ProgramName");

            migrationBuilder.RenameColumn(
                name: "ConsultantId",
                schema: "Travel",
                table: "Invoice",
                newName: "ProgramId");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                schema: "Travel",
                table: "Invoice",
                nullable: true);
        }
    }
}
