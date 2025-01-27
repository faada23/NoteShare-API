using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace API.Persistence.data.Migrations
{
    /// <inheritdoc />
    public partial class SeedAdd2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0000-000000000004"), "User" },
                    { new Guid("00000000-0000-0000-0000-000000000005"), "Moderator" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000005"));
        }
    }
}
