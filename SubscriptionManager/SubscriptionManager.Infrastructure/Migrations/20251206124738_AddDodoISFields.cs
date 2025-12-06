using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SubscriptionManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDodoISFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DodoISAccessToken",
                table: "AuthUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DodoISId",
                table: "AuthUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DodoISRefreshToken",
                table: "AuthUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DodoISTokenExpires",
                table: "AuthUsers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DodoISUnitId",
                table: "AuthUsers",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DodoISAccessToken",
                table: "AuthUsers");

            migrationBuilder.DropColumn(
                name: "DodoISId",
                table: "AuthUsers");

            migrationBuilder.DropColumn(
                name: "DodoISRefreshToken",
                table: "AuthUsers");

            migrationBuilder.DropColumn(
                name: "DodoISTokenExpires",
                table: "AuthUsers");

            migrationBuilder.DropColumn(
                name: "DodoISUnitId",
                table: "AuthUsers");
        }
    }
}
