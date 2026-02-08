using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Broker.Migrations
{
    /// <inheritdoc />
    public partial class AddDocumentVerificationStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Documents",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.Sql("UPDATE \"Documents\" SET \"Status\" = 2 WHERE \"IsVerified\" = TRUE;");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$L7qlRYhjdmNKJBxPiz3ULe.mh/HbrNKtHPIklyiu61E9e7EGMVogG");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "$2a$11$wg.WFGDNz6Zch89G3w7R5.UsqPnxG/Rq/v6Eyqs/gSMD3/aSzfcNO");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                column: "PasswordHash",
                value: "$2a$11$wg.WFGDNz6Zch89G3w7R5.UsqPnxG/Rq/v6Eyqs/gSMD3/aSzfcNO");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4,
                column: "PasswordHash",
                value: "$2a$11$wg.WFGDNz6Zch89G3w7R5.UsqPnxG/Rq/v6Eyqs/gSMD3/aSzfcNO");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 5,
                column: "PasswordHash",
                value: "$2a$11$0M8L4nMonWlzOQc2lkCtBO9ekYditMCzKORSWZ4z1rumsPt9yXbRa");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Documents");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$wFD7wAld1IlltO4diAX7p.5Tc4qeCh55YJKGhhmJg3qdr0MuvA.LO");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "$2a$11$CHxGnGtfev059rBFxeWlBun97e6cwKr1w6cDVxL.O/ehjmNOT3maS");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                column: "PasswordHash",
                value: "$2a$11$CHxGnGtfev059rBFxeWlBun97e6cwKr1w6cDVxL.O/ehjmNOT3maS");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4,
                column: "PasswordHash",
                value: "$2a$11$CHxGnGtfev059rBFxeWlBun97e6cwKr1w6cDVxL.O/ehjmNOT3maS");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 5,
                column: "PasswordHash",
                value: "$2a$11$1S.76j5pYs.gKAesmcT/gupp6N5Zok6Ku0JLnyortcNEfFu.EVTv6");
        }
    }
}
