using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrackYourLife.Modules.GuitarTabs.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGuitarTabYoutubeUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "YoutubeUrl",
                schema: "GuitarTabs",
                table: "SavedGuitarTab",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "YoutubeUrl",
                schema: "GuitarTabs",
                table: "SavedGuitarTab");
        }
    }
}
