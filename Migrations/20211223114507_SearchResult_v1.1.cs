using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IGApi.Migrations
{
    public partial class SearchResult_v11 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_search_result_epic_detail_epic",
                table: "search_result");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey(
                name: "FK_search_result_epic_detail_epic",
                table: "search_result",
                column: "epic",
                principalTable: "epic_detail",
                principalColumn: "epic",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
