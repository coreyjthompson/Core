using Microsoft.EntityFrameworkCore.Migrations;

namespace MEI.Core.Infrastructure.Data.Migrations
{
    public partial class AddedSortOrdertoTravelServiceModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                schema: "Travel",
                table: "Service",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SortOrder",
                schema: "Travel",
                table: "Service");
        }
    }
}
