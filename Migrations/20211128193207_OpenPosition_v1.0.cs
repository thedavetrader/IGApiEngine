using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IGApi.Migrations
{
    public partial class OpenPosition_v10 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "open_position",
                columns: table => new
                {
                    account_id = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    deal_id = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    created_date_utc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    contract_size = table.Column<decimal>(type: "decimal(38,19)", precision: 38, scale: 19, nullable: true),
                    size = table.Column<decimal>(type: "decimal(38,19)", precision: 38, scale: 19, nullable: true),
                    direction = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    limit_level = table.Column<decimal>(type: "decimal(38,19)", precision: 38, scale: 19, nullable: true),
                    level = table.Column<decimal>(type: "decimal(38,19)", precision: 38, scale: 19, nullable: true),
                    currency = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    controlled_risk = table.Column<bool>(type: "bit", nullable: false),
                    stop_level = table.Column<decimal>(type: "decimal(38,19)", precision: 38, scale: 19, nullable: true),
                    trailing_step = table.Column<decimal>(type: "decimal(38,19)", precision: 38, scale: 19, nullable: true),
                    trailing_stop_distance = table.Column<decimal>(type: "decimal(38,19)", precision: 38, scale: 19, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_open_position", x => new { x.account_id, x.deal_id });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "open_position");
        }
    }
}
