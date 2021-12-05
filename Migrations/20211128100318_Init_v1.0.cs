using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IGApi.Migrations
{
    public partial class Init_v10 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("SqlServer:MemoryOptimized", true);

            migrationBuilder.CreateTable(
                name: "account",
                columns: table => new
                {
                    account_id = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    account_name = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    account_alias = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    status = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    account_type = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    preferred = table.Column<bool>(type: "bit", nullable: true),
                    currency = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    can_transfer_from = table.Column<bool>(type: "bit", nullable: true),
                    can_transfer_to = table.Column<bool>(type: "bit", nullable: true),
                    deposit = table.Column<decimal>(type: "decimal(38,19)", precision: 38, scale: 19, nullable: true),
                    profit_and_loss = table.Column<decimal>(type: "decimal(38,19)", precision: 38, scale: 19, nullable: true),
                    equity = table.Column<decimal>(type: "decimal(38,19)", precision: 38, scale: 19, nullable: true),
                    equity_used = table.Column<decimal>(type: "decimal(38,19)", precision: 38, scale: 19, nullable: true),
                    used_margin = table.Column<decimal>(type: "decimal(38,19)", precision: 38, scale: 19, nullable: true),
                    amount_due = table.Column<decimal>(type: "decimal(38,19)", precision: 38, scale: 19, nullable: true),
                    available_cash = table.Column<decimal>(type: "decimal(38,19)", precision: 38, scale: 19, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_account", x => x.account_id);
                });

            migrationBuilder.CreateTable(
                name: "rest_request_queue",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    timestamp = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getutcdate()"),
                    rest_request = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    parameter = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    execute_asap = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    is_recurrent = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rest_request_queue", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                    table.CheckConstraint("CK_rest_request_queue_CHK_ig_rest_api_queue_rest_request", "rest_request in ('GetAccountDetails', 'GetOpenPositions', 'CreatePosition')");
                })
                .Annotation("SqlServer:MemoryOptimized", true);

            migrationBuilder.CreateIndex(
                name: "IX_rest_request_queue_execute_asap_timestamp",
                table: "rest_request_queue",
                columns: new[] { "execute_asap", "timestamp" })
                .Annotation("SqlServer:Clustered", false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "account");

            migrationBuilder.DropTable(
                name: "rest_request_queue")
                .Annotation("SqlServer:MemoryOptimized", true);

            migrationBuilder.AlterDatabase()
                .OldAnnotation("SqlServer:MemoryOptimized", true);
        }
    }
}
