using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IGApiEngine.Migrations
{
    public partial class NamingConvention : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "profitLoss",
                table: "AccountBalance",
                newName: "ProfitLoss");

            migrationBuilder.RenameColumn(
                name: "deposit",
                table: "AccountBalance",
                newName: "Deposit");

            migrationBuilder.RenameColumn(
                name: "balance",
                table: "AccountBalance",
                newName: "Balance");

            migrationBuilder.RenameColumn(
                name: "available",
                table: "AccountBalance",
                newName: "Available");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProfitLoss",
                table: "AccountBalance",
                newName: "profitLoss");

            migrationBuilder.RenameColumn(
                name: "Deposit",
                table: "AccountBalance",
                newName: "deposit");

            migrationBuilder.RenameColumn(
                name: "Balance",
                table: "AccountBalance",
                newName: "balance");

            migrationBuilder.RenameColumn(
                name: "Available",
                table: "AccountBalance",
                newName: "available");
        }
    }
}
