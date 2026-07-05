using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RealEstateSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRoleSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "RoleId", "NameRole" },
                values: new object[,]
                {
                    { new Guid("a5e2f5b8-5f2b-426c-941f-897b6a18d1f8"), "Admin" },
                    { new Guid("b1f5d6c8-2e8b-4a5c-9c7d-3a1b8c2d4e6f"), "Owner" },
                    { new Guid("c2d6e7f8-3a9b-4c5d-8b7a-9a1b8c2d3e4f"), "User" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("a5e2f5b8-5f2b-426c-941f-897b6a18d1f8"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("b1f5d6c8-2e8b-4a5c-9c7d-3a1b8c2d4e6f"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("c2d6e7f8-3a9b-4c5d-8b7a-9a1b8c2d3e4f"));
        }
    }
}
