using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrackYourLife.Modules.Youtube.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddYoutubeSettingsPassword : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SettingsPasswordHash",
                schema: "Youtube",
                table: "YoutubeSettings",
                type: "character varying(512)",
                maxLength: 512,
                nullable: true);

            migrationBuilder.DropColumn(
                name: "SettingsChangeFrequency",
                schema: "Youtube",
                table: "YoutubeSettings");

            migrationBuilder.DropColumn(
                name: "DaysBetweenChanges",
                schema: "Youtube",
                table: "YoutubeSettings");

            migrationBuilder.DropColumn(
                name: "LastSettingsChangeUtc",
                schema: "Youtube",
                table: "YoutubeSettings");

            migrationBuilder.DropColumn(
                name: "SpecificDayOfWeek",
                schema: "Youtube",
                table: "YoutubeSettings");

            migrationBuilder.DropColumn(
                name: "SpecificDayOfMonth",
                schema: "Youtube",
                table: "YoutubeSettings");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SettingsPasswordHash",
                schema: "Youtube",
                table: "YoutubeSettings");

            migrationBuilder.AddColumn<int>(
                name: "SettingsChangeFrequency",
                schema: "Youtube",
                table: "YoutubeSettings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DaysBetweenChanges",
                schema: "Youtube",
                table: "YoutubeSettings",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastSettingsChangeUtc",
                schema: "Youtube",
                table: "YoutubeSettings",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SpecificDayOfWeek",
                schema: "Youtube",
                table: "YoutubeSettings",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SpecificDayOfMonth",
                schema: "Youtube",
                table: "YoutubeSettings",
                type: "integer",
                nullable: true);
        }
    }
}
