using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IGApi.Migrations
{
    public partial class EpicDetail_v15 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "slippage_factor_unit",
                table: "epic_detail",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "slippage_factor_value",
                table: "epic_detail",
                type: "decimal(38,19)",
                precision: 38,
                scale: 19,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "slippage_factor_unit",
                table: "epic_detail");

            migrationBuilder.DropColumn(
                name: "slippage_factor_value",
                table: "epic_detail");
        }
    }
}
