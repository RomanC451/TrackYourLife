using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrackYourLife.Modules.Youtube.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddYoutubeChannelAndVideos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Youtube");

            migrationBuilder.CreateTable(
                name: "OutboxMessages",
                schema: "Youtube",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    RetryCount = table.Column<int>(type: "integer", nullable: false),
                    IsDirectEvent = table.Column<bool>(type: "boolean", nullable: false),
                    Error = table.Column<string>(type: "text", nullable: true),
                    OccurredOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProcessedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxMessages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "YoutubeChannel",
                schema: "Youtube",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    YoutubeChannelId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ThumbnailUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Category = table.Column<int>(type: "integer", nullable: false),
                    CreatedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YoutubeChannel", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_YoutubeChannel_UserId_Category",
                schema: "Youtube",
                table: "YoutubeChannel",
                columns: new[] { "UserId", "Category" });

            migrationBuilder.CreateIndex(
                name: "IX_YoutubeChannel_UserId_YoutubeChannelId",
                schema: "Youtube",
                table: "YoutubeChannel",
                columns: new[] { "UserId", "YoutubeChannelId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OutboxMessages",
                schema: "Youtube");

            migrationBuilder.DropTable(
                name: "YoutubeChannel",
                schema: "Youtube");
        }
    }
}
