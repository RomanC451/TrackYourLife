using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrackYourLife.Modules.Trainings.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CorrectNameforExerciseHistoryTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExercisesSetsHistory_Exercise_ExerciseId",
                schema: "Trainings",
                table: "ExercisesSetsHistory"
            );

            migrationBuilder.DropPrimaryKey(
                name: "PK_ExercisesSetsHistory",
                schema: "Trainings",
                table: "ExercisesSetsHistory"
            );

            migrationBuilder.RenameTable(
                name: "ExercisesSetsHistory",
                schema: "Trainings",
                newName: "ExerciseHistory",
                newSchema: "Trainings"
            );

            migrationBuilder.RenameIndex(
                name: "IX_ExercisesSetsHistory_ExerciseId",
                schema: "Trainings",
                table: "ExerciseHistory",
                newName: "IX_ExerciseHistory_ExerciseId"
            );

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExerciseHistory",
                schema: "Trainings",
                table: "ExerciseHistory",
                column: "Id"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_ExerciseHistory_Exercise_ExerciseId",
                schema: "Trainings",
                table: "ExerciseHistory",
                column: "ExerciseId",
                principalSchema: "Trainings",
                principalTable: "Exercise",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExerciseHistory_Exercise_ExerciseId",
                schema: "Trainings",
                table: "ExerciseHistory"
            );

            migrationBuilder.DropPrimaryKey(
                name: "PK_ExerciseHistory",
                schema: "Trainings",
                table: "ExerciseHistory"
            );

            migrationBuilder.RenameTable(
                name: "ExerciseHistory",
                schema: "Trainings",
                newName: "ExercisesSetsHistory",
                newSchema: "Trainings"
            );

            migrationBuilder.RenameIndex(
                name: "IX_ExerciseHistory_ExerciseId",
                schema: "Trainings",
                table: "ExercisesSetsHistory",
                newName: "IX_ExercisesSetsHistory_ExerciseId"
            );

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExercisesSetsHistory",
                schema: "Trainings",
                table: "ExercisesSetsHistory",
                column: "Id"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_ExercisesSetsHistory_Exercise_ExerciseId",
                schema: "Trainings",
                table: "ExercisesSetsHistory",
                column: "ExerciseId",
                principalSchema: "Trainings",
                principalTable: "Exercise",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );
        }
    }
}
