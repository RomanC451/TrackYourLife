using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrackYourLife.Modules.Trainings.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddExerciseHistoryEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExercisesSetsHistory",
                schema: "Trainings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ExerciseId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExerciseSetChangesJson = table.Column<string>(type: "text", nullable: false),
                    ExerciseSetsBeforeChangeJson = table.Column<string>(type: "text", nullable: false),
                    AreChangesApplied = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExercisesSetsHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExercisesSetsHistory_Exercise_ExerciseId",
                        column: x => x.ExerciseId,
                        principalSchema: "Trainings",
                        principalTable: "Exercise",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExercisesSetsHistory_ExerciseId",
                schema: "Trainings",
                table: "ExercisesSetsHistory",
                column: "ExerciseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExercisesSetsHistory",
                schema: "Trainings");
        }
    }
}
