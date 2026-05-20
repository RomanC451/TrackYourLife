using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrackYourLife.Modules.Youtube.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddYoutubeCategoriesAndDailyCategoryWatchCounters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "DailyEntertainmentCounter", schema: "Youtube");

            migrationBuilder.DropIndex(
                name: "IX_YoutubeChannel_UserId_Category",
                schema: "Youtube",
                table: "YoutubeChannel"
            );

            migrationBuilder.DropColumn(
                name: "MaxEntertainmentVideosPerDay",
                schema: "Youtube",
                table: "YoutubeSettings"
            );

            migrationBuilder.DropColumn(
                name: "Category",
                schema: "Youtube",
                table: "YoutubeChannel"
            );

            migrationBuilder.AddColumn<string>(
                name: "CategoryName",
                schema: "Youtube",
                table: "YoutubeChannel",
                type: "character varying(120)",
                maxLength: 120,
                nullable: false,
                defaultValue: ""
            );

            migrationBuilder.AddColumn<Guid>(
                name: "YoutubeCategoryId",
                schema: "Youtube",
                table: "YoutubeChannel",
                type: "uuid",
                nullable: false,
                defaultValue: Guid.Empty
            );

            migrationBuilder.CreateTable(
                name: "DailyCategoryWatchCounter",
                schema: "Youtube",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    YoutubeCategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    VideosWatchedCount = table.Column<int>(type: "integer", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyCategoryWatchCounter", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "YoutubeCategory",
                schema: "Youtube",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(
                        type: "character varying(120)",
                        maxLength: 120,
                        nullable: false
                    ),
                    MaxVideosPerDay = table.Column<int>(type: "integer", nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    CreatedOnUtc = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    ModifiedOnUtc = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YoutubeCategory", x => x.Id);
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_YoutubeChannel_UserId_YoutubeCategoryId",
                schema: "Youtube",
                table: "YoutubeChannel",
                columns: new[] { "UserId", "YoutubeCategoryId" }
            );

            migrationBuilder.CreateIndex(
                name: "IX_DailyCategoryWatchCounter_UserId",
                schema: "Youtube",
                table: "DailyCategoryWatchCounter",
                column: "UserId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_DailyCategoryWatchCounter_UserId_Date_YoutubeCategoryId",
                schema: "Youtube",
                table: "DailyCategoryWatchCounter",
                columns: new[] { "UserId", "Date", "YoutubeCategoryId" },
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_YoutubeCategory_UserId",
                schema: "Youtube",
                table: "YoutubeCategory",
                column: "UserId"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "DailyCategoryWatchCounter", schema: "Youtube");

            migrationBuilder.DropTable(name: "YoutubeCategory", schema: "Youtube");

            migrationBuilder.DropIndex(
                name: "IX_YoutubeChannel_UserId_YoutubeCategoryId",
                schema: "Youtube",
                table: "YoutubeChannel"
            );

            migrationBuilder.DropColumn(
                name: "CategoryName",
                schema: "Youtube",
                table: "YoutubeChannel"
            );

            migrationBuilder.DropColumn(
                name: "YoutubeCategoryId",
                schema: "Youtube",
                table: "YoutubeChannel"
            );

            migrationBuilder.AddColumn<int>(
                name: "MaxEntertainmentVideosPerDay",
                schema: "Youtube",
                table: "YoutubeSettings",
                type: "integer",
                nullable: false,
                defaultValue: 0
            );

            migrationBuilder.AddColumn<int>(
                name: "Category",
                schema: "Youtube",
                table: "YoutubeChannel",
                type: "integer",
                nullable: false,
                defaultValue: 0
            );

            migrationBuilder.CreateTable(
                name: "DailyEntertainmentCounter",
                schema: "Youtube",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    VideosWatchedCount = table.Column<int>(type: "integer", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyEntertainmentCounter", x => x.Id);
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_YoutubeChannel_UserId_Category",
                schema: "Youtube",
                table: "YoutubeChannel",
                columns: new[] { "UserId", "Category" }
            );

            migrationBuilder.CreateIndex(
                name: "IX_DailyEntertainmentCounter_UserId",
                schema: "Youtube",
                table: "DailyEntertainmentCounter",
                column: "UserId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_DailyEntertainmentCounter_UserId_Date",
                schema: "Youtube",
                table: "DailyEntertainmentCounter",
                columns: new[] { "UserId", "Date" },
                unique: true
            );
        }
    }
}
