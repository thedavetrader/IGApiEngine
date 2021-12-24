using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IGApi.Migrations
{
    public partial class init : Migration
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
                name: "activity_history",
                columns: table => new
                {
                    deal_id = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    epic = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    activity_history_id = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    activity = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    market_name = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    period = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    result = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    channel = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    currency = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    size = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    level = table.Column<decimal>(type: "decimal(38,19)", precision: 38, scale: 19, nullable: true),
                    stop = table.Column<decimal>(type: "decimal(38,19)", precision: 38, scale: 19, nullable: true),
                    stop_type = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    limit = table.Column<decimal>(type: "decimal(38,19)", precision: 38, scale: 19, nullable: true),
                    action_status = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    reference = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_activity_history", x => new { x.timestamp, x.deal_id });
                });

            migrationBuilder.CreateTable(
                name: "api_engine_status",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    is_alive = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_api_engine_status", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                })
                .Annotation("SqlServer:MemoryOptimized", true);

            migrationBuilder.CreateTable(
                name: "api_request_queue_item",
                columns: table => new
                {
                    guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "newid()"),
                    parent_guid = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    timestamp = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getutcdate()"),
                    request = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    parameter = table.Column<string>(type: "nvarchar(max)", maxLength: 4000, nullable: true),
                    execute_asap = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    is_recurrent = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    is_running = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_api_request_queue_item", x => x.guid)
                        .Annotation("SqlServer:Clustered", false);
                    table.CheckConstraint("CK_api_request_queue_item_request", "request in ('GetAccountDetails','GetOpenPositions','GetWorkingOrders','GetEpicDetails','GetActivityHistory','GetTransactionHistory','GetClientSentiment','CreatePosition','EditPosition','ClosePosition','CreateWorkingOrder','EditWorkingOrder','DeleteWorkingOrder','GetWatchlists','CreateWatchlist','DeleteWatchlist','GetWatchListEpics','AddWatchlistEpic','RemoveWatchlistEpic','Search')");
                })
                .Annotation("SqlServer:MemoryOptimized", true);

            migrationBuilder.CreateTable(
                name: "client_sentiment",
                columns: table => new
                {
                    market_id = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    long_position_percentage = table.Column<decimal>(type: "decimal(38,19)", precision: 38, scale: 19, nullable: true),
                    short_position_percentage = table.Column<decimal>(type: "decimal(38,19)", precision: 38, scale: 19, nullable: true),
                    api_last_update = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_client_sentiment", x => x.market_id);
                });

            migrationBuilder.CreateTable(
                name: "confirm_response",
                columns: table => new
                {
                    timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    deal_reference = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    status = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    reason = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    deal_status = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    epic = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    expiry = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    deal_id = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    level = table.Column<decimal>(type: "decimal(38,19)", precision: 38, scale: 19, nullable: true),
                    size = table.Column<decimal>(type: "decimal(38,19)", precision: 38, scale: 19, nullable: true),
                    direction = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    stop_level = table.Column<decimal>(type: "decimal(38,19)", precision: 38, scale: 19, nullable: true),
                    limit_level = table.Column<decimal>(type: "decimal(38,19)", precision: 38, scale: 19, nullable: true),
                    stop_distance = table.Column<decimal>(type: "decimal(38,19)", precision: 38, scale: 19, nullable: true),
                    limit_distance = table.Column<decimal>(type: "decimal(38,19)", precision: 38, scale: 19, nullable: true),
                    guaranteed_stop = table.Column<bool>(type: "bit", nullable: false),
                    affected_deals = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_confirm_response", x => new { x.timestamp, x.deal_reference })
                        .Annotation("SqlServer:Clustered", false);
                })
                .Annotation("SqlServer:MemoryOptimized", true);

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
                    market_id = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    sprintmarket_minimum_expiry_time = table.Column<int>(type: "int", nullable: false),
                    sprintmarket_maximum_expiry_time = table.Column<int>(type: "int", nullable: false),
                    margin_factor = table.Column<decimal>(type: "decimal(38,19)", precision: 38, scale: 19, nullable: true),
                    margin_factor_unit = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    news_code = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    chart_code = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    country = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    value_of_one_pip = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    one_pip_means = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    contract_size = table.Column<decimal>(type: "decimal(38,19)", precision: 38, scale: 19, nullable: true),
                    api_last_update = table.Column<DateTime>(type: "datetime2", nullable: false),
                    expiry_last_dealingdate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    expiry_settlement_info = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    slippage_factor_unit = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    slippage_factor_value = table.Column<decimal>(type: "decimal(38,19)", precision: 38, scale: 19, nullable: true),
                    last_rollover_time = table.Column<DateTime>(type: "datetime2", nullable: true),
                    rollover_info = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    dealing_rule_unit_min_step_distance = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    dealing_rule_unit_min_deal_size = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    dealing_rule_unit_min_controlled_risk_stop_distance = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    dealing_rule_unit_min_normal_stop_or_limit_distance = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    dealing_rule_unit_max_stop_or_limit_distance = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    dealing_rule_value_min_step_distance = table.Column<decimal>(type: "decimal(38,19)", precision: 38, scale: 19, nullable: true),
                    dealing_rule_value_min_deal_size = table.Column<decimal>(type: "decimal(38,19)", precision: 38, scale: 19, nullable: true),
                    dealing_rule_value_min_controlled_risk_stop_distance = table.Column<decimal>(type: "decimal(38,19)", precision: 38, scale: 19, nullable: true),
                    dealing_rule_value_min_normal_stop_or_limit_distance = table.Column<decimal>(type: "decimal(38,19)", precision: 38, scale: 19, nullable: true),
                    dealing_rule_value_max_stop_or_limit_distance = table.Column<decimal>(type: "decimal(38,19)", precision: 38, scale: 19, nullable: true),
                    dealing_rule_market_order_preference = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    dealing_rule_trailing_stop_preference = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true)
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
                    deal_id = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    epic = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    deal_reference = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
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
                name: "search_result",
                columns: table => new
                {
                    epic = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_search_result", x => x.epic);
                });

            migrationBuilder.CreateTable(
                name: "transaction_history",
                columns: table => new
                {
                    date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    reference = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    instrument_name = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    period = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    profit_and_loss = table.Column<decimal>(type: "decimal(38,19)", precision: 38, scale: 19, nullable: false),
                    transaction_type = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    open_level = table.Column<decimal>(type: "decimal(38,19)", precision: 38, scale: 19, nullable: true),
                    close_level = table.Column<decimal>(type: "decimal(38,19)", precision: 38, scale: 19, nullable: true),
                    size = table.Column<decimal>(type: "decimal(38,19)", precision: 38, scale: 19, nullable: true),
                    currency = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    cash_transaction = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_transaction_history", x => new { x.date, x.reference });
                });

            migrationBuilder.CreateTable(
                name: "watchlist",
                columns: table => new
                {
                    account_id = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    watchlist_id = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    name = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    editable = table.Column<bool>(type: "bit", nullable: false),
                    deletable = table.Column<bool>(type: "bit", nullable: false),
                    default_system_watchlist = table.Column<bool>(type: "bit", nullable: false),
                    api_last_update = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_watchlist", x => new { x.account_id, x.watchlist_id });
                });

            migrationBuilder.CreateTable(
                name: "working_order",
                columns: table => new
                {
                    account_id = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    deal_id = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    direction = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    epic = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    order_size = table.Column<decimal>(type: "decimal(38,19)", precision: 38, scale: 19, nullable: true),
                    order_level = table.Column<decimal>(type: "decimal(38,19)", precision: 38, scale: 19, nullable: true),
                    time_in_force = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    good_till_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    created_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    guaranteed_stop = table.Column<bool>(type: "bit", nullable: false),
                    order_type = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    stop_distance = table.Column<decimal>(type: "decimal(38,19)", precision: 38, scale: 19, nullable: true),
                    limit_distance = table.Column<decimal>(type: "decimal(38,19)", precision: 38, scale: 19, nullable: true),
                    currency_code = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    dma = table.Column<bool>(type: "bit", nullable: false),
                    api_last_update = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_working_order", x => new { x.account_id, x.deal_id });
                });

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

            migrationBuilder.CreateTable(
                name: "watchlist_epic_detail",
                columns: table => new
                {
                    account_id = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    watchlist_id = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    epic = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_watchlist_epic_detail", x => new { x.account_id, x.watchlist_id, x.epic });
                    table.ForeignKey(
                        name: "FK_watchlist_epic_detail_epic_detail_epic",
                        column: x => x.epic,
                        principalTable: "epic_detail",
                        principalColumn: "epic",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_watchlist_epic_detail_watchlist_account_id_watchlist_id",
                        columns: x => new { x.account_id, x.watchlist_id },
                        principalTable: "watchlist",
                        principalColumns: new[] { "account_id", "watchlist_id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_api_request_queue_item_execute_asap_timestamp",
                table: "api_request_queue_item",
                columns: new[] { "execute_asap", "timestamp" })
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.CreateIndex(
                name: "IX_epic_detail_currency_code",
                table: "epic_detail_currency",
                column: "code");

            migrationBuilder.CreateIndex(
                name: "IX_watchlist_epic_detail_epic",
                table: "watchlist_epic_detail",
                column: "epic");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "account");

            migrationBuilder.DropTable(
                name: "activity_history");

            migrationBuilder.DropTable(
                name: "api_engine_status")
                .Annotation("SqlServer:MemoryOptimized", true);

            migrationBuilder.DropTable(
                name: "api_request_queue_item")
                .Annotation("SqlServer:MemoryOptimized", true);

            migrationBuilder.DropTable(
                name: "client_sentiment");

            migrationBuilder.DropTable(
                name: "confirm_response")
                .Annotation("SqlServer:MemoryOptimized", true);

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
                name: "search_result");

            migrationBuilder.DropTable(
                name: "transaction_history");

            migrationBuilder.DropTable(
                name: "watchlist_epic_detail");

            migrationBuilder.DropTable(
                name: "working_order");

            migrationBuilder.DropTable(
                name: "currency");

            migrationBuilder.DropTable(
                name: "epic_detail");

            migrationBuilder.DropTable(
                name: "watchlist");

            migrationBuilder.AlterDatabase()
                .OldAnnotation("SqlServer:MemoryOptimized", true);
        }
    }
}
