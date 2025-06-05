using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrackYourLife.Modules.Trainings.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveEquipmentEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Exercise_Equipment_EquipmentId",
                schema: "Trainings",
                table: "Exercise");

            migrationBuilder.DropTable(
                name: "Equipment",
                schema: "Trainings");

            migrationBuilder.DropIndex(
                name: "IX_Exercise_EquipmentId",
                schema: "Trainings",
                table: "Exercise");

            migrationBuilder.DropColumn(
                name: "EquipmentId",
                schema: "Trainings",
                table: "Exercise");

            migrationBuilder.AddColumn<string>(
                name: "Equipment",
                schema: "Trainings",
                table: "Exercise",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Equipment",
                schema: "Trainings",
                table: "Exercise");

            migrationBuilder.AddColumn<Guid>(
                name: "EquipmentId",
                schema: "Trainings",
                table: "Exercise",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Equipment",
                schema: "Trainings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    PictureUrl = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    VideoUrl = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipment", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Exercise_EquipmentId",
                schema: "Trainings",
                table: "Exercise",
                column: "EquipmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Exercise_Equipment_EquipmentId",
                schema: "Trainings",
                table: "Exercise",
                column: "EquipmentId",
                principalSchema: "Trainings",
                principalTable: "Equipment",
                principalColumn: "Id");
        }
    }
}
