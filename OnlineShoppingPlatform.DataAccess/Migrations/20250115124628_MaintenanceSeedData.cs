using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineShoppingPlatform.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class MaintenanceSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "MaintenanceModes",
                columns: new[] { "MaintenanceId", "CreatedDate", "EndTime", "IsActive", "IsDeleted", "Message", "ModifiedDate", "StartTime" },
                values: new object[] { -1, new DateTime(2025, 1, 15, 15, 46, 28, 309, DateTimeKind.Local).AddTicks(6959), new DateTime(2025, 1, 15, 16, 46, 28, 309, DateTimeKind.Local).AddTicks(6962), true, false, "İlk kurulum bakımı (Varsayılan olarak)", null, new DateTime(2025, 1, 15, 15, 46, 28, 309, DateTimeKind.Local).AddTicks(6961) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "MaintenanceModes",
                keyColumn: "MaintenanceId",
                keyValue: -1);
        }
    }
}
