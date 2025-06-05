using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrackYourLife.Modules.Trainings.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameSetsToExerciseSetsOnExerciceEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SetsJson",
                schema: "Trainings",
                table: "Exercise",
                newName: "ExerciseSetsJson");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ExerciseSetsJson",
                schema: "Trainings",
                table: "Exercise",
                newName: "SetsJson");
        }
    }
}
