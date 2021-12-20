using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IGApi.Migrations
{
    public partial class ApiEngineStatus_v10 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "api_engine_status",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    is_alive = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_api_engine_status", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                })
                .Annotation("SqlServer:MemoryOptimized", true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "api_engine_status")
                .Annotation("SqlServer:MemoryOptimized", true);
        }
    }
}
