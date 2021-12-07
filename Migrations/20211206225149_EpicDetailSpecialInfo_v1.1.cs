using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IGApi.Migrations
{
    public partial class EpicDetailSpecialInfo_v11 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_epic_detail_special_info",
                table: "epic_detail_special_info");

            migrationBuilder.AddPrimaryKey(
                name: "PK_epic_detail_special_info",
                table: "epic_detail_special_info",
                columns: new[] { "epic", "special_info" });

            migrationBuilder.CreateIndex(
                name: "IX_epic_detail_special_info_epic",
                table: "epic_detail_special_info",
                column: "epic",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_epic_detail_special_info",
                table: "epic_detail_special_info");

            migrationBuilder.DropIndex(
                name: "IX_epic_detail_special_info_epic",
                table: "epic_detail_special_info");

            migrationBuilder.AddPrimaryKey(
                name: "PK_epic_detail_special_info",
                table: "epic_detail_special_info",
                column: "epic");
        }
    }
}
