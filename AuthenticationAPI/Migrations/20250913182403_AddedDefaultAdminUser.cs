using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthenticationAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddedDefaultAdminUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "Email", "PasswordHash", "RoleId", "UpdatedAt", "UpdatedBy", "Username" },
                values: new object[] { 1, new DateTime(2025, 8, 29, 12, 30, 0, 0, DateTimeKind.Utc), "System", null, "AQAAAAIAAYagAAAAEBFIfNPAOhEACrN97HkZI+2OUGGh/3Iaj/jhgBWBtD3wg33TeLw34dzgaJLIxjhfqw==", 2, new DateTime(2025, 8, 29, 12, 30, 0, 0, DateTimeKind.Utc), "System", "admin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
