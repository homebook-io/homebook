using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeBook.Backend.Data.PostgreSql.Migrations
{
    /// <inheritdoc />
    public partial class AddUserCreatedAndDisabled : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "Users",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "Disabled",
                table: "Users",
                type: "datetime2",
                nullable: true);

            // Set Created for all existing users to the current UTC timestamp
            migrationBuilder.Sql("UPDATE \"Users\" SET \"Created\" = NOW() AT TIME ZONE 'UTC' WHERE \"Created\" IS NULL OR \"Created\" = '0001-01-01 00:00:00'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Created",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Disabled",
                table: "Users");
        }
    }
}
