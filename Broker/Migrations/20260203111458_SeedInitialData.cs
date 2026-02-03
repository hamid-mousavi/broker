using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Broker.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "EmailVerificationToken", "FirstName", "IsActive", "IsVerified", "LastName", "PasswordHash", "PasswordResetToken", "PasswordResetTokenExpiry", "PhoneNumber", "ProfileImagePath", "RefreshToken", "RefreshTokenExpiryTime", "Role", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "admin@broker.ir", null, "ادمین", true, true, "سیستم", "$2a$11$.GJSZORAU3Rfb9i2XRX98.8v/8VPZ3ysdChnj8OtTd9JRMhHXIy.W", null, null, "09120000001", null, null, null, 3, null },
                    { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "mrezaei@broker.ir", null, "محمد", true, true, "رضایی", "$2a$11$BmiKzEQ8yIOrYrDplREAmOP4kLCiTeG7nbXv5M4SrrqsHQMSPIs9.", null, null, "09120000002", null, null, null, 1, null },
                    { 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "skazemi@broker.ir", null, "ساناز", true, true, "کاظمی", "$2a$11$BmiKzEQ8yIOrYrDplREAmOP4kLCiTeG7nbXv5M4SrrqsHQMSPIs9.", null, null, "09120000003", null, null, null, 1, null },
                    { 4, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "rhoseini@broker.ir", null, "رضا", true, true, "حسینی", "$2a$11$BmiKzEQ8yIOrYrDplREAmOP4kLCiTeG7nbXv5M4SrrqsHQMSPIs9.", null, null, "09120000004", null, null, null, 1, null },
                    { 5, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "esamadi@broker.ir", null, "الهام", true, true, "صمدی", "$2a$11$SoB3uAcYI1hdZ8TpW1ewceU04I9s5YDX65ZBFGtw6ImwM0wPwZxfW", null, null, "09120000005", null, null, null, 2, null }
                });

            migrationBuilder.InsertData(
                table: "CargoOwners",
                columns: new[] { "Id", "Address", "City", "CompanyName", "CreatedAt", "EconomicCode", "NationalId", "Province", "UpdatedAt", "UserId" },
                values: new object[] { 1, null, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, null, 5 });

            migrationBuilder.InsertData(
                table: "ClearanceAgents",
                columns: new[] { "Id", "Address", "AverageRating", "City", "CompanyName", "CompletedRequests", "CreatedAt", "Description", "IsVerified", "LicenseNumber", "PostalCode", "Province", "TotalRatings", "UpdatedAt", "UserId", "Website", "YearsOfExperience" },
                values: new object[,]
                {
                    { 1, null, 0m, "تهران", "شرکت ترخیص تهران", 0, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null, null, "تهران", 0, null, 2, null, 9 },
                    { 2, null, 0m, "بندرعباس", "ترخیص هرمزگان", 0, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null, null, "هرمزگان", 0, null, 3, null, 6 },
                    { 3, null, 0m, "چابهار", "ترخیص جنوب شرق", 0, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null, null, "سیستان و بلوچستان", 0, null, 4, null, 8 }
                });

            migrationBuilder.InsertData(
                table: "AgentSpecializations",
                columns: new[] { "Id", "AgentId", "SpecializationName" },
                values: new object[,]
                {
                    { 1, 1, "واردات" },
                    { 2, 1, "کالاهای صنعتی" },
                    { 3, 2, "صادرات" },
                    { 4, 2, "مواد غذایی" },
                    { 5, 3, "ترانزیت" },
                    { 6, 3, "کالاهای حجیم" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AgentSpecializations",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AgentSpecializations",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AgentSpecializations",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "AgentSpecializations",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "AgentSpecializations",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "AgentSpecializations",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "CargoOwners",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ClearanceAgents",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ClearanceAgents",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ClearanceAgents",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4);
        }
    }
}
