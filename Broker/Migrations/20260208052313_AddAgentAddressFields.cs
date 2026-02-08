using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Broker.Migrations
{
    /// <inheritdoc />
    public partial class AddAgentAddressFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LegalAddress",
                table: "ClearanceAgents",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PersonalAddress",
                table: "ClearanceAgents",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "ClearanceAgents",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "LegalAddress", "PersonalAddress" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "ClearanceAgents",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "LegalAddress", "PersonalAddress" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "ClearanceAgents",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "LegalAddress", "PersonalAddress" },
                values: new object[] { null, null });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LegalAddress",
                table: "ClearanceAgents");

            migrationBuilder.DropColumn(
                name: "PersonalAddress",
                table: "ClearanceAgents");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$Z3GpcIzttVQnl/qAEMhR.OqLGH9vMS6xEXpCAZnEVPjfPl5zp3MIq");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "$2a$11$zHrrJdcgkoBtER/bnlkkv.zHCdvsKZl6UjXjfA5SUX.pqvc7s/IZi");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                column: "PasswordHash",
                value: "$2a$11$zHrrJdcgkoBtER/bnlkkv.zHCdvsKZl6UjXjfA5SUX.pqvc7s/IZi");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4,
                column: "PasswordHash",
                value: "$2a$11$zHrrJdcgkoBtER/bnlkkv.zHCdvsKZl6UjXjfA5SUX.pqvc7s/IZi");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 5,
                column: "PasswordHash",
                value: "$2a$11$ImOTAi19B63TFfNodXx.9eZhLCkFd5hEPWZZ6gh1L0faYl9dVoyGG");
        }
    }
}
