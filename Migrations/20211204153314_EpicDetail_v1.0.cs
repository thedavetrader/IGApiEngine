using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IGApi.Migrations
{
    public partial class EpicDetail_v10 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "epic_detail",
                columns: table => new
                {
                    epic = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    expiry = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    name = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    force_open_allowed = table.Column<bool>(type: "bit", nullable: false),
                    stop_limit_allowed = table.Column<bool>(type: "bit", nullable: false),
                    lot_size = table.Column<decimal>(type: "decimal(38,19)", precision: 38, scale: 19, nullable: true),
                    position_size_unit = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    type = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    controlled_risk_allowed = table.Column<bool>(type: "bit", nullable: false),
                    streaming_prices_available = table.Column<bool>(type: "bit", nullable: false),
                    market_id = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    sprintmarket_minimum_expiry_time = table.Column<int>(type: "int", nullable: false),
                    sprintmarket_maximum_expiry_time = table.Column<int>(type: "int", nullable: false),
                    margin_factor = table.Column<decimal>(type: "decimal(38,19)", precision: 38, scale: 19, nullable: true),
                    margin_factor_unit = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    news_code = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    chart_code = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    country = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    value_of_one_pip = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    one_pip_means = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    contract_size = table.Column<decimal>(type: "decimal(38,19)", precision: 38, scale: 19, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_epic_detail", x => x.epic);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "epic_detail");
        }
    }
}
