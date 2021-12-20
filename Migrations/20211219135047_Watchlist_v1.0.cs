using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IGApi.Migrations
{
    public partial class Watchlist_v10 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "watchlist",
                columns: table => new
                {
                    account_id = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    id = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    name = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    editable = table.Column<bool>(type: "bit", nullable: false),
                    deletable = table.Column<bool>(type: "bit", nullable: false),
                    default_system_watchlist = table.Column<bool>(type: "bit", nullable: false),
                    api_last_update = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_watchlist", x => new { x.account_id, x.id });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "watchlist");
        }
    }
}
