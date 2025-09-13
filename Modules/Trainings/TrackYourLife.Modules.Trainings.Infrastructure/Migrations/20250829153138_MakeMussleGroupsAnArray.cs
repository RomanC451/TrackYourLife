using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrackYourLife.Modules.Trainings.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakeMussleGroupsAnArray : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MuscleGroup",
                schema: "Trainings",
                table: "Exercise");

            migrationBuilder.AddColumn<List<string>>(
                name: "MuscleGroups",
                schema: "Trainings",
                table: "Exercise",
                type: "text[]",
                nullable: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MuscleGroups",
                schema: "Trainings",
                table: "Exercise");

            migrationBuilder.AddColumn<string>(
                name: "MuscleGroup",
                schema: "Trainings",
                table: "Exercise",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
