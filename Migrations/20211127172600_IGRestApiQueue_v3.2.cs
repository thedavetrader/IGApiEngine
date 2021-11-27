using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IGApiEngine.Migrations
{
    public partial class IGRestApiQueue_v32 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddCheckConstraint(
                name: "CK_IGRestApiQueue_CHK_IGRestApiQueue_RestRequest",
                table: "IGRestApiQueue",
                sql: "RestRequest in ('GetAccountDetails', 'GetOpenPositions', 'CreatePosition')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_IGRestApiQueue_CHK_IGRestApiQueue_RestRequest",
                table: "IGRestApiQueue");
        }
    }
}
