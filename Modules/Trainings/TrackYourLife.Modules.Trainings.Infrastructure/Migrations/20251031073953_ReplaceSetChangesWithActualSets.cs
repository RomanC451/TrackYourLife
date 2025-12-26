using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrackYourLife.Modules.Trainings.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceSetChangesWithActualSets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ExerciseSetsBeforeChangeJson",
                schema: "Trainings",
                table: "ExerciseHistory",
                newName: "OldExerciseSetsJson");

            migrationBuilder.RenameColumn(
                name: "ExerciseSetChangesJson",
                schema: "Trainings",
                table: "ExerciseHistory",
                newName: "NewExerciseSetsJson");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OldExerciseSetsJson",
                schema: "Trainings",
                table: "ExerciseHistory",
                newName: "ExerciseSetsBeforeChangeJson");

            migrationBuilder.RenameColumn(
                name: "NewExerciseSetsJson",
                schema: "Trainings",
                table: "ExerciseHistory",
                newName: "ExerciseSetChangesJson");
        }
    }
}
