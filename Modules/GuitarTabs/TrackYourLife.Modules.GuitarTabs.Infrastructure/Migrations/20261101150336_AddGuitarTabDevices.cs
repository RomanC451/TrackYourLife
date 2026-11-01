using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrackYourLife.Modules.GuitarTabs.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGuitarTabDevices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GuitarTabDevice",
                schema: "GuitarTabs",
                columns: table => new
                {
                    DeviceId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Speed = table.Column<decimal>(type: "numeric(4,1)", precision: 4, scale: 1, nullable: false),
                    Semitones = table.Column<int>(type: "integer", nullable: false),
                    UseFlats = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuitarTabDevice", x => x.DeviceId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GuitarTabDevice_UserId",
                schema: "GuitarTabs",
                table: "GuitarTabDevice",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_GuitarTabDevice_UserId_Name",
                schema: "GuitarTabs",
                table: "GuitarTabDevice",
                columns: new[] { "UserId", "Name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GuitarTabDevice",
                schema: "GuitarTabs");
        }
    }
}
