using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace HomeBook.Backend.Data.Mysql.Migrations
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
                type: "datetime2(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified))
                .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<DateTime>(
                name: "Disabled",
                table: "Users",
                type: "datetime2(6)",
                nullable: true);

            // Set Created for all existing users to the current UTC timestamp
            migrationBuilder.Sql("UPDATE Users SET Created = UTC_TIMESTAMP() WHERE Created IS NULL OR Created = '0001-01-01 00:00:00'");
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
