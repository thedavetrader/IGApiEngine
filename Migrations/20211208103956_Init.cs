using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IGApi.Migrations
{
    public partial class Init : Migration
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
                    api_last_update = table.Column<DateTime>(type: "datetime2", nullable: false),
                    account_name = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    account_alias = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    status = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    account_type = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    preferred = table.Column<bool>(type: "bit", nullable: true),
                    currency = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    can_transfer_from = table.Column<bool>(type: "bit", nullable: true),
                    can_transfer_to = table.Column<bool>(type: "bit", nullable: true),
                    balance = table.Column<decimal>(type: "decimal(38,19)", precision: 38, scale: 19, nullable: true),
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
                name: "currency",
                columns: table => new
                {
                    code = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    symbol = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    base_exchange_rate = table.Column<decimal>(type: "decimal(38,19)", precision: 38, scale: 19, nullable: true),
                    is_default = table.Column<bool>(type: "bit", nullable: false),
                    api_last_update = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_currency", x => x.code);
                });

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
                    slippage_factor_unit = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    slippage_factor_value = table.Column<decimal>(type: "decimal(38,19)", precision: 38, scale: 19, nullable: true),
                    expiry_last_dealingdate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    expiry_settlement_info = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    last_rollover_time = table.Column<DateTime>(type: "datetime2", nullable: true),
                    rollover_info = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    news_code = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    chart_code = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    country = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    value_of_one_pip = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    one_pip_means = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    contract_size = table.Column<decimal>(type: "decimal(38,19)", precision: 38, scale: 19, nullable: true),
                    api_last_update = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_epic_detail", x => x.epic);
                });

            migrationBuilder.CreateTable(
                name: "epic_tick",
                columns: table => new
                {
                    epic = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    mid_open = table.Column<decimal>(type: "decimal(38,19)", precision: 38, scale: 19, nullable: true),
                    high = table.Column<decimal>(type: "decimal(38,19)", precision: 38, scale: 19, nullable: true),
                    low = table.Column<decimal>(type: "decimal(38,19)", precision: 38, scale: 19, nullable: true),
                    change = table.Column<decimal>(type: "decimal(38,19)", precision: 38, scale: 19, nullable: true),
                    change_percentage = table.Column<decimal>(type: "decimal(38,19)", precision: 38, scale: 19, nullable: true),
                    update_time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    market_delay = table.Column<int>(type: "int", nullable: true),
                    market_state = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    bid = table.Column<decimal>(type: "decimal(38,19)", precision: 38, scale: 19, nullable: true),
                    offer = table.Column<decimal>(type: "decimal(38,19)", precision: 38, scale: 19, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_epic_tick", x => x.epic)
                        .Annotation("SqlServer:Clustered", false);
                })
                .Annotation("SqlServer:MemoryOptimized", true);

            migrationBuilder.CreateTable(
                name: "open_position",
                columns: table => new
                {
                    account_id = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    deal_id = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    epic = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    created_date_utc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    contract_size = table.Column<decimal>(type: "decimal(38,19)", precision: 38, scale: 19, nullable: true),
                    size = table.Column<decimal>(type: "decimal(38,19)", precision: 38, scale: 19, nullable: true),
                    direction = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    limit_level = table.Column<decimal>(type: "decimal(38,19)", precision: 38, scale: 19, nullable: true),
                    level = table.Column<decimal>(type: "decimal(38,19)", precision: 38, scale: 19, nullable: true),
                    currency = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    controlled_risk = table.Column<bool>(type: "bit", nullable: false),
                    stop_level = table.Column<decimal>(type: "decimal(38,19)", precision: 38, scale: 19, nullable: true),
                    trailing_step = table.Column<decimal>(type: "decimal(38,19)", precision: 38, scale: 19, nullable: true),
                    trailing_stop_distance = table.Column<decimal>(type: "decimal(38,19)", precision: 38, scale: 19, nullable: true),
                    api_last_update = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_open_position", x => new { x.account_id, x.deal_id });
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
                    table.CheckConstraint("CK_rest_request_queue_rest_request", "rest_request in ('GetAccountDetails', 'GetOpenPositions', 'CreatePosition','GetEpicDetails')");
                })
                .Annotation("SqlServer:MemoryOptimized", true);

            migrationBuilder.CreateTable(
                name: "epic_detail_currency",
                columns: table => new
                {
                    epic = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    code = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    api_last_update = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_epic_detail_currency", x => new { x.epic, x.code });
                    table.ForeignKey(
                        name: "FK_epic_detail_currency_currency_code",
                        column: x => x.code,
                        principalTable: "currency",
                        principalColumn: "code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_epic_detail_currency_epic_detail_epic",
                        column: x => x.epic,
                        principalTable: "epic_detail",
                        principalColumn: "epic",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "epic_detail_margin_deposit_band",
                columns: table => new
                {
                    epic = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    currency = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    min = table.Column<decimal>(type: "decimal(38,19)", precision: 38, scale: 19, nullable: false),
                    max = table.Column<decimal>(type: "decimal(38,19)", precision: 38, scale: 19, nullable: true),
                    margin = table.Column<decimal>(type: "decimal(38,19)", precision: 38, scale: 19, nullable: true),
                    api_last_update = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_epic_detail_margin_deposit_band", x => new { x.epic, x.currency, x.min });
                    table.ForeignKey(
                        name: "FK_epic_detail_margin_deposit_band_epic_detail_epic",
                        column: x => x.epic,
                        principalTable: "epic_detail",
                        principalColumn: "epic",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "epic_detail_opening_hour",
                columns: table => new
                {
                    epic = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    open_time = table.Column<TimeSpan>(type: "time", nullable: false),
                    close_time = table.Column<TimeSpan>(type: "time", nullable: false),
                    api_last_update = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_epic_detail_opening_hour", x => new { x.epic, x.open_time });
                    table.ForeignKey(
                        name: "FK_epic_detail_opening_hour_epic_detail_epic",
                        column: x => x.epic,
                        principalTable: "epic_detail",
                        principalColumn: "epic",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "epic_detail_special_info",
                columns: table => new
                {
                    epic = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    special_info = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    api_last_update = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_epic_detail_special_info", x => new { x.epic, x.special_info });
                    table.ForeignKey(
                        name: "FK_epic_detail_special_info_epic_detail_epic",
                        column: x => x.epic,
                        principalTable: "epic_detail",
                        principalColumn: "epic",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_epic_detail_currency_code",
                table: "epic_detail_currency",
                column: "code");

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
                name: "epic_detail_currency");

            migrationBuilder.DropTable(
                name: "epic_detail_margin_deposit_band");

            migrationBuilder.DropTable(
                name: "epic_detail_opening_hour");

            migrationBuilder.DropTable(
                name: "epic_detail_special_info");

            migrationBuilder.DropTable(
                name: "epic_tick")
                .Annotation("SqlServer:MemoryOptimized", true);

            migrationBuilder.DropTable(
                name: "open_position");

            migrationBuilder.DropTable(
                name: "rest_request_queue")
                .Annotation("SqlServer:MemoryOptimized", true);

            migrationBuilder.DropTable(
                name: "currency");

            migrationBuilder.DropTable(
                name: "epic_detail");

            migrationBuilder.AlterDatabase()
                .OldAnnotation("SqlServer:MemoryOptimized", true);
        }
    }
}
