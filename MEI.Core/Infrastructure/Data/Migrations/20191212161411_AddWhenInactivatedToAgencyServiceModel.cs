using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MEI.Core.Infrastructure.Data.Migrations
{
    public partial class AddWhenInactivatedToAgencyServiceModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "WhenInactivated",
                schema: "Travel",
                table: "AgencyService",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WhenInactivated",
                schema: "Travel",
                table: "AgencyService");
        }
    }
}
