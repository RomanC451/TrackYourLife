using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrackYourLife.Modules.Youtube.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPlaylistEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "YoutubePlaylist",
                schema: "Youtube",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CreatedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YoutubePlaylist", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "YoutubePlaylistVideo",
                schema: "Youtube",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    YoutubePlaylistId = table.Column<Guid>(type: "uuid", nullable: false),
                    VideoId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    AddedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YoutubePlaylistVideo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_YoutubePlaylistVideo_YoutubePlaylist_YoutubePlaylistId",
                        column: x => x.YoutubePlaylistId,
                        principalSchema: "Youtube",
                        principalTable: "YoutubePlaylist",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_YoutubePlaylist_UserId",
                schema: "Youtube",
                table: "YoutubePlaylist",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_YoutubePlaylistVideo_YoutubePlaylistId_VideoId",
                schema: "Youtube",
                table: "YoutubePlaylistVideo",
                columns: new[] { "YoutubePlaylistId", "VideoId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "YoutubePlaylistVideo",
                schema: "Youtube");

            migrationBuilder.DropTable(
                name: "YoutubePlaylist",
                schema: "Youtube");
        }
    }
}
