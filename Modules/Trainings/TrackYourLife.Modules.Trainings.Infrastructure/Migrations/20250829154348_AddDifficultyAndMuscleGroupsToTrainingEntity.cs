using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrackYourLife.Modules.Trainings.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDifficultyAndMuscleGroupsToTrainingEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Difficulty",
                schema: "Trainings",
                table: "Training",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<List<string>>(
                name: "MuscleGroups",
                schema: "Trainings",
                table: "Training",
                type: "text[]",
                nullable: false);

            migrationBuilder.AddColumn<int>(
                name: "Difficulty",
                schema: "Trainings",
                table: "Exercise",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Difficulty",
                schema: "Trainings",
                table: "Training");

            migrationBuilder.DropColumn(
                name: "MuscleGroups",
                schema: "Trainings",
                table: "Training");

            migrationBuilder.DropColumn(
                name: "Difficulty",
                schema: "Trainings",
                table: "Exercise");
        }
    }
}
