using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrackYourLife.Modules.Users.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSubscriptionFieldsToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PlanType",
                schema: "Users",
                table: "Users",
                type: "character varying(16)",
                maxLength: 16,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StripeCustomerId",
                schema: "Users",
                table: "Users",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SubscriptionEndsAtUtc",
                schema: "Users",
                table: "Users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubscriptionStatus",
                schema: "Users",
                table: "Users",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PlanType",
                schema: "Users",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "StripeCustomerId",
                schema: "Users",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SubscriptionEndsAtUtc",
                schema: "Users",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SubscriptionStatus",
                schema: "Users",
                table: "Users");
        }
    }
}
