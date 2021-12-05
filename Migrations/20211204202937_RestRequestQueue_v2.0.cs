using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IGApi.Migrations
{
    public partial class RestRequestQueue_v20 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_rest_request_queue_CHK_ig_rest_api_queue_rest_request",
                table: "rest_request_queue");

            migrationBuilder.AddCheckConstraint(
                name: "CK_rest_request_queue_rest_request",
                table: "rest_request_queue",
                sql: "rest_request in ('GetAccountDetails', 'GetOpenPositions', 'CreatePosition''GetEpicDetails')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_rest_request_queue_rest_request",
                table: "rest_request_queue");

            migrationBuilder.AddCheckConstraint(
                name: "CK_rest_request_queue_CHK_ig_rest_api_queue_rest_request",
                table: "rest_request_queue",
                sql: "rest_request in ('GetAccountDetails', 'GetOpenPositions', 'CreatePosition')");
        }
    }
}
