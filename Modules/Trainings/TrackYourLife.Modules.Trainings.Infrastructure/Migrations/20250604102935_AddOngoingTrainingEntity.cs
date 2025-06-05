using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrackYourLife.Modules.Trainings.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOngoingTrainingEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OngoingTraining",
                schema: "Trainings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    TrainingId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExerciseIndex = table.Column<int>(type: "integer", nullable: false),
                    SetIndex = table.Column<int>(type: "integer", nullable: false),
                    StartedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FinishedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OngoingTraining", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OngoingTraining_Training_TrainingId",
                        column: x => x.TrainingId,
                        principalSchema: "Trainings",
                        principalTable: "Training",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OngoingTraining_TrainingId",
                schema: "Trainings",
                table: "OngoingTraining",
                column: "TrainingId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OngoingTraining",
                schema: "Trainings");
        }
    }
}
