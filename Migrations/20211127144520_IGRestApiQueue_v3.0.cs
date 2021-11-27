using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IGApiEngine.Migrations
{
    public partial class IGRestApiQueue_v30 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ExecuteImmediately",
                table: "IGRestApiQueue",
                newName: "ExecuteAsap");

            migrationBuilder.RenameIndex(
                name: "IX_IGRestApiQueue_ExecuteImmediately_Timestamp",
                table: "IGRestApiQueue",
                newName: "IX_IGRestApiQueue_ExecuteAsap_Timestamp");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ExecuteAsap",
                table: "IGRestApiQueue",
                newName: "ExecuteImmediately");

            migrationBuilder.RenameIndex(
                name: "IX_IGRestApiQueue_ExecuteAsap_Timestamp",
                table: "IGRestApiQueue",
                newName: "IX_IGRestApiQueue_ExecuteImmediately_Timestamp");
        }
    }
}
