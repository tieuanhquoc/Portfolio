using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MainApp.Migrations
{
    public partial class u2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "AboutMe", "Adress", "Avatar", "CreatedAt", "Email", "Password", "Phone", "Role", "Status", "Username" },
                values: new object[] { 2, "No thing", "Địa chỉ", "", new DateTime(2022, 5, 26, 0, 46, 36, 783, DateTimeKind.Local).AddTicks(5424), "user@yobmail.com", "AQAAAAEAACcQAAAAEAdfOiMfprvHLFijGOQVDzxP1xW4aQQfM0FdMFsLSW7su95STBT0eVil1hTnHdDQCg==", "0982456799", 2, 1, "user" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
