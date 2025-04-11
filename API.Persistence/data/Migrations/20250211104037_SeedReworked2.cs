using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace API.Persistence.data.Migrations
{
    /// <inheritdoc />
    public partial class SeedReworked2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "IsBanned", "PasswordHash", "Username" },
                values: new object[,]
                {
                    { new Guid("a1b2c3d4-e5f6-4a1b-2c3d-4e5f6a1b2c3d"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "AQAAAAIAAYagAAAAEFf8EZDaZNR9N5r66K5zpnbJ2lY3Ud4jPfj47A0gHBXY/wuAbbB5y2cM5aVsmlIcMQ==", "moder2" },
                    { new Guid("f42b3e7d-9c5a-4b1f-8b3e-7d9c5a4b1f8b"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "AQAAAAIAAYagAAAAEDKeGlx3GBKx0sij9+aGvKW5q/BWYLB7jeyWhYFavnz8MM8Kb0KO6wXnuEhwKwA3Qw==", "moder1" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-e5f6-4a1b-2c3d-4e5f6a1b2c3d"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("f42b3e7d-9c5a-4b1f-8b3e-7d9c5a4b1f8b"));
        }
    }
}
