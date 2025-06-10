using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrackYourLife.Modules.Trainings.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOngoingTrainingIdToExerciseHistoryEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OngoingTrainingId",
                schema: "Trainings",
                table: "ExerciseHistory",
                type: "uuid",
                nullable: false,
                defaultValue: Guid.Empty
            );

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseHistory_OngoingTrainingId",
                schema: "Trainings",
                table: "ExerciseHistory",
                column: "OngoingTrainingId"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_ExerciseHistory_OngoingTraining_OngoingTrainingId",
                schema: "Trainings",
                table: "ExerciseHistory",
                column: "OngoingTrainingId",
                principalSchema: "Trainings",
                principalTable: "OngoingTraining",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExerciseHistory_OngoingTraining_OngoingTrainingId",
                schema: "Trainings",
                table: "ExerciseHistory"
            );

            migrationBuilder.DropIndex(
                name: "IX_ExerciseHistory_OngoingTrainingId",
                schema: "Trainings",
                table: "ExerciseHistory"
            );

            migrationBuilder.DropColumn(
                name: "OngoingTrainingId",
                schema: "Trainings",
                table: "ExerciseHistory"
            );
        }
    }
}
