﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IGApi.Migrations
{
    public partial class Watchlist_v11 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_api_request_queue_item_request",
                table: "api_request_queue_item");

            migrationBuilder.AddCheckConstraint(
                name: "CK_api_request_queue_item_request",
                table: "api_request_queue_item",
                sql: "request in ('GetAccountDetails','GetOpenPositions','GetWorkingOrders','GetActivityHistory','GetTransactionHistory','GetClientSentiment','CreatePosition','EditPosition','ClosePosition','CreateWorkingOrder','EditWorkingOrder','DeleteWorkingOrder','GetWatchlists','GetEpicDetails')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_api_request_queue_item_request",
                table: "api_request_queue_item");

            migrationBuilder.AddCheckConstraint(
                name: "CK_api_request_queue_item_request",
                table: "api_request_queue_item",
                sql: "request in ('GetAccountDetails','GetOpenPositions','GetWorkingOrders','GetActivityHistory','GetTransactionHistory','GetClientSentiment','CreatePosition','EditPosition','ClosePosition','CreateWorkingOrder','EditWorkingOrder','DeleteWorkingOrder','GetEpicDetails')");
        }
    }
}
