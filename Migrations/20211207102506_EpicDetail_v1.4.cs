using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IGApi.Migrations
{
    public partial class EpicDetail_v14 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "last_rollover_time",
                table: "epic_detail",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "rollover_info",
                table: "epic_detail",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "last_rollover_time",
                table: "epic_detail");

            migrationBuilder.DropColumn(
                name: "rollover_info",
                table: "epic_detail");
        }
    }
}
