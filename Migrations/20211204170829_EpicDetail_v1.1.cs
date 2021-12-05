using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IGApi.Migrations
{
    public partial class EpicDetail_v11 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "epic_detail_special_info",
                columns: table => new
                {
                    epic = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    special_info = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_epic_detail_special_info", x => x.epic);
                    table.ForeignKey(
                        name: "FK_epic_detail_special_info_epic_detail_epic",
                        column: x => x.epic,
                        principalTable: "epic_detail",
                        principalColumn: "epic",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "epic_detail_special_info");
        }
    }
}
