using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrackYourLife.Modules.GuitarTabs.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGuitarPlaylistSongs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SavedGuitarTab",
                schema: "GuitarTabs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    TabUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    SongName = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    ArtistName = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Type = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    IsFavorite = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavedGuitarTab", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GuitarPlaylistSong",
                schema: "GuitarTabs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GuitarPlaylistId = table.Column<Guid>(type: "uuid", nullable: false),
                    SavedGuitarTabId = table.Column<Guid>(type: "uuid", nullable: false),
                    AddedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuitarPlaylistSong", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GuitarPlaylistSong_GuitarPlaylist_GuitarPlaylistId",
                        column: x => x.GuitarPlaylistId,
                        principalSchema: "GuitarTabs",
                        principalTable: "GuitarPlaylist",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GuitarPlaylistSong_SavedGuitarTab_SavedGuitarTabId",
                        column: x => x.SavedGuitarTabId,
                        principalSchema: "GuitarTabs",
                        principalTable: "SavedGuitarTab",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GuitarPlaylistSong_GuitarPlaylistId_SavedGuitarTabId",
                schema: "GuitarTabs",
                table: "GuitarPlaylistSong",
                columns: new[] { "GuitarPlaylistId", "SavedGuitarTabId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GuitarPlaylistSong_SavedGuitarTabId",
                schema: "GuitarTabs",
                table: "GuitarPlaylistSong",
                column: "SavedGuitarTabId");

            migrationBuilder.CreateIndex(
                name: "IX_SavedGuitarTab_UserId_TabUrl",
                schema: "GuitarTabs",
                table: "SavedGuitarTab",
                columns: new[] { "UserId", "TabUrl" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GuitarPlaylistSong",
                schema: "GuitarTabs");

            migrationBuilder.DropTable(
                name: "SavedGuitarTab",
                schema: "GuitarTabs");
        }
    }
}
