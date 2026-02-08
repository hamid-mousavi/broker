using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Broker.Migrations
{
    /// <inheritdoc />
    public partial class AddAgentLegalEntityFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EconomicCode",
                table: "ClearanceAgents",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsLegalEntity",
                table: "ClearanceAgents",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "NationalId",
                table: "ClearanceAgents",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegistrationNumber",
                table: "ClearanceAgents",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "ClearanceAgents",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "EconomicCode", "IsLegalEntity", "NationalId", "RegistrationNumber" },
                values: new object[] { null, false, null, null });

            migrationBuilder.UpdateData(
                table: "ClearanceAgents",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "EconomicCode", "IsLegalEntity", "NationalId", "RegistrationNumber" },
                values: new object[] { null, false, null, null });

            migrationBuilder.UpdateData(
                table: "ClearanceAgents",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "EconomicCode", "IsLegalEntity", "NationalId", "RegistrationNumber" },
                values: new object[] { null, false, null, null });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EconomicCode",
                table: "ClearanceAgents");

            migrationBuilder.DropColumn(
                name: "IsLegalEntity",
                table: "ClearanceAgents");

            migrationBuilder.DropColumn(
                name: "NationalId",
                table: "ClearanceAgents");

            migrationBuilder.DropColumn(
                name: "RegistrationNumber",
                table: "ClearanceAgents");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$.GJSZORAU3Rfb9i2XRX98.8v/8VPZ3ysdChnj8OtTd9JRMhHXIy.W");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "$2a$11$BmiKzEQ8yIOrYrDplREAmOP4kLCiTeG7nbXv5M4SrrqsHQMSPIs9.");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                column: "PasswordHash",
                value: "$2a$11$BmiKzEQ8yIOrYrDplREAmOP4kLCiTeG7nbXv5M4SrrqsHQMSPIs9.");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4,
                column: "PasswordHash",
                value: "$2a$11$BmiKzEQ8yIOrYrDplREAmOP4kLCiTeG7nbXv5M4SrrqsHQMSPIs9.");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 5,
                column: "PasswordHash",
                value: "$2a$11$SoB3uAcYI1hdZ8TpW1ewceU04I9s5YDX65ZBFGtw6ImwM0wPwZxfW");
        }
    }
}
