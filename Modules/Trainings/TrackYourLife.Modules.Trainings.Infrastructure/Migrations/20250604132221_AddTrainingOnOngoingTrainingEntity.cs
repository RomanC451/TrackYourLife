using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrackYourLife.Modules.Trainings.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTrainingOnOngoingTrainingEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_TrainingExercise_ExerciseId",
                schema: "Trainings",
                table: "TrainingExercise",
                column: "ExerciseId");

            migrationBuilder.AddForeignKey(
                name: "FK_TrainingExercise_Exercise_ExerciseId",
                schema: "Trainings",
                table: "TrainingExercise",
                column: "ExerciseId",
                principalSchema: "Trainings",
                principalTable: "Exercise",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrainingExercise_Exercise_ExerciseId",
                schema: "Trainings",
                table: "TrainingExercise");

            migrationBuilder.DropIndex(
                name: "IX_TrainingExercise_ExerciseId",
                schema: "Trainings",
                table: "TrainingExercise");
        }
    }
}
