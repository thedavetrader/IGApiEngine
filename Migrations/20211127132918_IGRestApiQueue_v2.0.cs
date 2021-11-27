using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IGApiEngine.Migrations
{
    public partial class IGRestApiQueue_v20 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IGRestApiQueue")
                .Annotation("SqlServer:MemoryOptimized", true);

            migrationBuilder.AlterDatabase()
                .OldAnnotation("SqlServer:MemoryOptimized", true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("SqlServer:MemoryOptimized", true);

            migrationBuilder.CreateTable(
                name: "IGRestApiQueue",
                columns: table => new
                {
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getutcdate()"),
                    RestRequest = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    ExecuteImmediately = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsRecurrent = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Parameters = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IGRestApiQueue", x => new { x.Timestamp, x.RestRequest })
                        .Annotation("SqlServer:Clustered", false);
                })
                .Annotation("SqlServer:MemoryOptimized", true);

            migrationBuilder.CreateIndex(
                name: "IX_IGRestApiQueue_ExecuteImmediately_Timestamp",
                table: "IGRestApiQueue",
                columns: new[] { "ExecuteImmediately", "Timestamp" })
                .Annotation("SqlServer:Clustered", false);
        }
    }
}
