using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IGApi.Migrations
{
    public partial class RemoveOneToOneRelations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_epic_detail_special_info_epic",
                table: "epic_detail_special_info");

            migrationBuilder.DropIndex(
                name: "IX_epic_detail_margin_deposit_band_epic",
                table: "epic_detail_margin_deposit_band");

            migrationBuilder.DropIndex(
                name: "IX_epic_detail_currency_epic",
                table: "epic_detail_currency");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_epic_detail_special_info_epic",
                table: "epic_detail_special_info",
                column: "epic",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_epic_detail_margin_deposit_band_epic",
                table: "epic_detail_margin_deposit_band",
                column: "epic",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_epic_detail_currency_epic",
                table: "epic_detail_currency",
                column: "epic",
                unique: true);
        }
    }
}
