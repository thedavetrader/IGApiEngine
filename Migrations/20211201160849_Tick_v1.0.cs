using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IGApi.Migrations
{
    public partial class Tick_v10 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tick",
                columns: table => new
                {
                    epic = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    mid_open = table.Column<decimal>(type: "decimal(38,19)", precision: 38, scale: 19, nullable: true),
                    high = table.Column<decimal>(type: "decimal(38,19)", precision: 38, scale: 19, nullable: true),
                    low = table.Column<decimal>(type: "decimal(38,19)", precision: 38, scale: 19, nullable: true),
                    change = table.Column<decimal>(type: "decimal(38,19)", precision: 38, scale: 19, nullable: true),
                    change_percentage = table.Column<decimal>(type: "decimal(38,19)", precision: 38, scale: 19, nullable: true),
                    update_time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    market_delay = table.Column<int>(type: "int", nullable: true),
                    market_state = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    bid = table.Column<decimal>(type: "decimal(38,19)", precision: 38, scale: 19, nullable: true),
                    offer = table.Column<decimal>(type: "decimal(38,19)", precision: 38, scale: 19, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tick", x => x.epic)
                        .Annotation("SqlServer:Clustered", false);
                })
                .Annotation("SqlServer:MemoryOptimized", true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tick")
                .Annotation("SqlServer:MemoryOptimized", true);
        }
    }
}
