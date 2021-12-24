using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IGApi.Migrations
{
    public partial class Search_v10 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_api_request_queue_item_request",
                table: "api_request_queue_item");

            migrationBuilder.AddCheckConstraint(
                name: "CK_api_request_queue_item_request",
                table: "api_request_queue_item",
                sql: "request in ('GetAccountDetails','GetOpenPositions','GetWorkingOrders','GetEpicDetails','GetActivityHistory','GetTransactionHistory','GetClientSentiment','CreatePosition','EditPosition','ClosePosition','CreateWorkingOrder','EditWorkingOrder','DeleteWorkingOrder','GetWatchlists','CreateWatchlist','DeleteWatchlist','GetWatchListEpics','AddWatchlistEpic','RemoveWatchlistEpic','Search')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_api_request_queue_item_request",
                table: "api_request_queue_item");

            migrationBuilder.AddCheckConstraint(
                name: "CK_api_request_queue_item_request",
                table: "api_request_queue_item",
                sql: "request in ('GetAccountDetails','GetOpenPositions','GetWorkingOrders','GetEpicDetails','GetActivityHistory','GetTransactionHistory','GetClientSentiment','CreatePosition','EditPosition','ClosePosition','CreateWorkingOrder','EditWorkingOrder','DeleteWorkingOrder','GetWatchlists','CreateWatchlist','DeleteWatchlist','GetWatchListEpics','AddWatchlistEpic','RemoveWatchlistEpic')");
        }
    }
}
