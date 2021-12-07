using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IGApi.Migrations
{
    public partial class EpicDetailCurrency_v10 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "epic_detail_currency",
                columns: table => new
                {
                    epic = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    code = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    symbol = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    base_exchange_rate = table.Column<decimal>(type: "decimal(38,19)", precision: 38, scale: 19, nullable: true),
                    is_default = table.Column<bool>(type: "bit", nullable: false),
                    api_last_update = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_epic_detail_currency", x => new { x.epic, x.code });
                    table.ForeignKey(
                        name: "FK_epic_detail_currency_epic_detail_epic",
                        column: x => x.epic,
                        principalTable: "epic_detail",
                        principalColumn: "epic",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_epic_detail_currency_epic",
                table: "epic_detail_currency",
                column: "epic",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "epic_detail_currency");
        }
    }
}
