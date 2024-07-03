using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BookStoreApp.API.Migrations
{
    /// <inheritdoc />
    public partial class SeededDefaultUsersAndRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "55dc6311-e149-41a5-9f45-bb3ca5e99eee", null, "Administrator", "ADMINISTRATOR" },
                    { "a56d0879-dbf0-4e18-ae87-2a64d2269314", null, "User", "USER" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "25749bc2-d43e-4643-8060-fef24bd93df6", 0, "95db92b1-85b5-4fac-96a3-53e5fcf42047", "admin@bookstore.com", false, "System", "Admin", false, null, "ADMIN@BOOKSTORE.COM", "ADMIN@BOOKSTORE.COM", "AQAAAAIAAYagAAAAEN4HrK4c808Cm9dR0gMEPaow7CNBJ6KmoR//TnjcwlnRmgZWF2vyt5G+cacO7OXNcQ==", null, false, "36140a12-ae0b-461b-9404-7adbd44810a8", false, "admin@bookstore.com" },
                    { "39dab0b5-1fb0-4907-974a-ab901de45cf6", 0, "138dcd94-4625-49be-9f37-d355b2ca6c83", "user@bookstore.com", false, "System", "User", false, null, "USER@BOOKSTORE.COM", "USER@BOOKSTORE.COM", "AQAAAAIAAYagAAAAEA4mVgVbKSWCgDduI/wTZRAsb2GTzPPjNodDXwRJjUPiA8huOqHxkEOFDGg73AH4YQ==", null, false, "9737201c-30e3-412a-9a18-eacc34de3276", false, "user@bookstore.com" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { "55dc6311-e149-41a5-9f45-bb3ca5e99eee", "25749bc2-d43e-4643-8060-fef24bd93df6" },
                    { "a56d0879-dbf0-4e18-ae87-2a64d2269314", "39dab0b5-1fb0-4907-974a-ab901de45cf6" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "55dc6311-e149-41a5-9f45-bb3ca5e99eee", "25749bc2-d43e-4643-8060-fef24bd93df6" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "a56d0879-dbf0-4e18-ae87-2a64d2269314", "39dab0b5-1fb0-4907-974a-ab901de45cf6" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "55dc6311-e149-41a5-9f45-bb3ca5e99eee");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a56d0879-dbf0-4e18-ae87-2a64d2269314");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "25749bc2-d43e-4643-8060-fef24bd93df6");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "39dab0b5-1fb0-4907-974a-ab901de45cf6");
        }
    }
}
