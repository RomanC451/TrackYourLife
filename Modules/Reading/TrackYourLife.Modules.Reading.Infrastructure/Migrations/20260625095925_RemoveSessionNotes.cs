using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrackYourLife.Modules.Reading.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSessionNotes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Notes",
                schema: "Reading",
                table: "ReadingSessions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Notes",
                schema: "Reading",
                table: "ReadingSessions",
                type: "character varying(4000)",
                maxLength: 4000,
                nullable: true);
        }
    }
}
