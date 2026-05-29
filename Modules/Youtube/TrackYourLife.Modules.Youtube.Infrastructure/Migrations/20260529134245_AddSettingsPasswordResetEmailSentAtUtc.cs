using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrackYourLife.Modules.Youtube.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSettingsPasswordResetEmailSentAtUtc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "SettingsPasswordResetEmailSentAtUtc",
                schema: "Youtube",
                table: "YoutubeSettings",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SettingsPasswordResetEmailSentAtUtc",
                schema: "Youtube",
                table: "YoutubeSettings");
        }
    }
}
