using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrackYourLife.Modules.Trainings.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ConvertSetsTableToJsonColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Set",
                schema: "Trainings");

            migrationBuilder.AddColumn<string>(
                name: "SetsJson",
                schema: "Trainings",
                table: "Exercise",
                type: "jsonb",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SetsJson",
                schema: "Trainings",
                table: "Exercise");

            migrationBuilder.CreateTable(
                name: "Set",
                schema: "Trainings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ExerciseId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Reps = table.Column<int>(type: "integer", nullable: false),
                    Weight = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Set", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Set_Exercise_ExerciseId",
                        column: x => x.ExerciseId,
                        principalSchema: "Trainings",
                        principalTable: "Exercise",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Set_ExerciseId",
                schema: "Trainings",
                table: "Set",
                column: "ExerciseId");
        }
    }
}
