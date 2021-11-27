using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IGApiEngine.Migrations
{
    public partial class IGApiAccountBalance_IGApiAccountDetails_IGApiStreamingAccountData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccountBalance",
                columns: table => new
                {
                    AccountId = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    balance = table.Column<decimal>(type: "decimal(38,19)", precision: 38, scale: 19, nullable: true),
                    deposit = table.Column<decimal>(type: "decimal(38,19)", precision: 38, scale: 19, nullable: true),
                    profitLoss = table.Column<decimal>(type: "decimal(38,19)", precision: 38, scale: 19, nullable: true),
                    available = table.Column<decimal>(type: "decimal(38,19)", precision: 38, scale: 19, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountBalance", x => x.AccountId);
                });

            migrationBuilder.CreateTable(
                name: "AccountDetails",
                columns: table => new
                {
                    accountId = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    accountName = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    accountAlias = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    status = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    accountType = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    preferred = table.Column<bool>(type: "bit", nullable: false),
                    currency = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    canTransferFrom = table.Column<bool>(type: "bit", nullable: false),
                    canTransferTo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountDetails", x => x.accountId);
                });

            migrationBuilder.CreateTable(
                name: "StreamingAccountData",
                columns: table => new
                {
                    AccountId = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(38,19)", precision: 38, scale: 19, nullable: true),
                    Equity = table.Column<decimal>(type: "decimal(38,19)", precision: 38, scale: 19, nullable: true),
                    EquityUsed = table.Column<decimal>(type: "decimal(38,19)", precision: 38, scale: 19, nullable: true),
                    ProfitAndLoss = table.Column<decimal>(type: "decimal(38,19)", precision: 38, scale: 19, nullable: true),
                    Deposit = table.Column<decimal>(type: "decimal(38,19)", precision: 38, scale: 19, nullable: true),
                    UsedMargin = table.Column<decimal>(type: "decimal(38,19)", precision: 38, scale: 19, nullable: true),
                    AmountDue = table.Column<decimal>(type: "decimal(38,19)", precision: 38, scale: 19, nullable: true),
                    AvailableCash = table.Column<decimal>(type: "decimal(38,19)", precision: 38, scale: 19, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StreamingAccountData", x => x.AccountId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountBalance");

            migrationBuilder.DropTable(
                name: "AccountDetails");

            migrationBuilder.DropTable(
                name: "StreamingAccountData");
        }
    }
}
