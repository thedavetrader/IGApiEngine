using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IGApi.Migrations
{
    public partial class ApiRequestQueueItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "rest_request_queue")
                .Annotation("SqlServer:MemoryOptimized", true);

            migrationBuilder.CreateTable(
                name: "api_request_queue_item",
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
                    table.PrimaryKey("PK_api_request_queue_item", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                    table.CheckConstraint("CK_api_request_queue_item_rest_request", "rest_request in ('GetAccountDetails','GetOpenPositions','GetWorkingOrders','GetActivityHistory','GetTransactionHistory','GetClientSentiment','CreatePosition','EditPosition','ClosePosition','CreateWorkingOrder','EditWorkingOrder','DeleteWorkingOrder','GetEpicDetails')");
                })
                .Annotation("SqlServer:MemoryOptimized", true);

            migrationBuilder.CreateIndex(
                name: "IX_api_request_queue_item_execute_asap_timestamp",
                table: "api_request_queue_item",
                columns: new[] { "execute_asap", "timestamp" })
                .Annotation("SqlServer:Clustered", false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "api_request_queue_item")
                .Annotation("SqlServer:MemoryOptimized", true);

            migrationBuilder.CreateTable(
                name: "rest_request_queue",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    execute_asap = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    is_recurrent = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    parameter = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    rest_request = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    timestamp = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getutcdate()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rest_request_queue", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                    table.CheckConstraint("CK_rest_request_queue_rest_request", "rest_request in ('GetAccountDetails','GetOpenPositions','GetWorkingOrders','GetActivityHistory','GetTransactionHistory','GetClientSentiment','CreatePosition','EditPosition','ClosePosition','CreateWorkingOrder','EditWorkingOrder','DeleteWorkingOrder','GetEpicDetails')");
                })
                .Annotation("SqlServer:MemoryOptimized", true);

            migrationBuilder.CreateIndex(
                name: "IX_rest_request_queue_execute_asap_timestamp",
                table: "rest_request_queue",
                columns: new[] { "execute_asap", "timestamp" })
                .Annotation("SqlServer:Clustered", false);
        }
    }
}
