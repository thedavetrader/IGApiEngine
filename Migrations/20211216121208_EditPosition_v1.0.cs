using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IGApi.Migrations
{
    public partial class EditPosition_v10 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_rest_request_queue_rest_request",
                table: "rest_request_queue");

            migrationBuilder.AddCheckConstraint(
                name: "CK_rest_request_queue_rest_request",
                table: "rest_request_queue",
                sql: "rest_request in ('GetAccountDetails','GetOpenPositions','GetWorkingOrders','GetActivityHistory','GetTransactionHistory','GetClientSentiment','CreatePosition','EditPosition','ClosePosition','GetEpicDetails')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_rest_request_queue_rest_request",
                table: "rest_request_queue");

            migrationBuilder.AddCheckConstraint(
                name: "CK_rest_request_queue_rest_request",
                table: "rest_request_queue",
                sql: "rest_request in ('GetAccountDetails','GetOpenPositions','GetWorkingOrders','GetActivityHistory','GetTransactionHistory','GetClientSentiment','CreatePosition','ClosePosition','GetEpicDetails')");
        }
    }
}
