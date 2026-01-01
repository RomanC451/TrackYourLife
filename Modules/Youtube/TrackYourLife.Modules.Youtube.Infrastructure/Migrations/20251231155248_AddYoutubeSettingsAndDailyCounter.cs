using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrackYourLife.Modules.Youtube.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddYoutubeSettingsAndDailyCounter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DailyDivertissmentCounter",
                schema: "Youtube",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    VideosWatchedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyDivertissmentCounter", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WatchedVideo",
                schema: "Youtube",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    VideoId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ChannelId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    WatchedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WatchedVideo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "YoutubeSettings",
                schema: "Youtube",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    MaxDivertissmentVideosPerDay = table.Column<int>(type: "integer", nullable: false),
                    SettingsChangeFrequency = table.Column<int>(type: "integer", nullable: false),
                    DaysBetweenChanges = table.Column<int>(type: "integer", nullable: true),
                    LastSettingsChangeUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SpecificDayOfWeek = table.Column<int>(type: "integer", nullable: true),
                    SpecificDayOfMonth = table.Column<int>(type: "integer", nullable: true),
                    CreatedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YoutubeSettings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DailyDivertissmentCounter_UserId",
                schema: "Youtube",
                table: "DailyDivertissmentCounter",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_DailyDivertissmentCounter_UserId_Date",
                schema: "Youtube",
                table: "DailyDivertissmentCounter",
                columns: new[] { "UserId", "Date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WatchedVideo_UserId",
                schema: "Youtube",
                table: "WatchedVideo",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_WatchedVideo_UserId_VideoId",
                schema: "Youtube",
                table: "WatchedVideo",
                columns: new[] { "UserId", "VideoId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_YoutubeSettings_UserId",
                schema: "Youtube",
                table: "YoutubeSettings",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DailyDivertissmentCounter",
                schema: "Youtube");

            migrationBuilder.DropTable(
                name: "WatchedVideo",
                schema: "Youtube");

            migrationBuilder.DropTable(
                name: "YoutubeSettings",
                schema: "Youtube");
        }
    }
}
