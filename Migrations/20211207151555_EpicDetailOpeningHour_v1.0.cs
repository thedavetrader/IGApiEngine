using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IGApi.Migrations
{
    public partial class EpicDetailOpeningHour_v10 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "epic_detail_opening_hour",
                columns: table => new
                {
                    epic = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    open_time = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    close_time = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    api_last_update = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_epic_detail_opening_hour", x => new { x.epic, x.open_time });
                    table.ForeignKey(
                        name: "FK_epic_detail_opening_hour_epic_detail_epic",
                        column: x => x.epic,
                        principalTable: "epic_detail",
                        principalColumn: "epic",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "epic_detail_opening_hour");
        }
    }
}
