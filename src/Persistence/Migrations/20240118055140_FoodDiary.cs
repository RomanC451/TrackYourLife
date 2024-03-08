using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrackYourLifeDotnet.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FoodDiary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AddedAt",
                table: "FoodDiary",
                newName: "CreatedOnUtc");

            migrationBuilder.AddColumn<DateOnly>(
                name: "Date",
                table: "FoodDiary",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOnUtc",
                table: "FoodDiary",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Date",
                table: "FoodDiary");

            migrationBuilder.DropColumn(
                name: "ModifiedOnUtc",
                table: "FoodDiary");

            migrationBuilder.RenameColumn(
                name: "CreatedOnUtc",
                table: "FoodDiary",
                newName: "AddedAt");
        }
    }
}
