using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrackYourLife.Modules.Trainings.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ConvertSetsTableToJsonColumn2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Exercise_Equipment_EquipmentId",
                schema: "Trainings",
                table: "Exercise"
            );

            migrationBuilder.AlterColumn<string>(
                name: "VideoUrl",
                schema: "Trainings",
                table: "Exercise",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text"
            );

            migrationBuilder.AlterColumn<string>(
                name: "PictureUrl",
                schema: "Trainings",
                table: "Exercise",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text"
            );

            migrationBuilder.AlterColumn<Guid>(
                name: "EquipmentId",
                schema: "Trainings",
                table: "Exercise",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid"
            );

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "Trainings",
                table: "Exercise",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Exercise_Equipment_EquipmentId",
                schema: "Trainings",
                table: "Exercise",
                column: "EquipmentId",
                principalSchema: "Trainings",
                principalTable: "Equipment",
                principalColumn: "Id"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Exercise_Equipment_EquipmentId",
                schema: "Trainings",
                table: "Exercise"
            );

            migrationBuilder.AlterColumn<string>(
                name: "VideoUrl",
                schema: "Trainings",
                table: "Exercise",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<string>(
                name: "PictureUrl",
                schema: "Trainings",
                table: "Exercise",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<Guid>(
                name: "EquipmentId",
                schema: "Trainings",
                table: "Exercise",
                type: "uuid",
                nullable: false,
                defaultValue: Guid.Empty,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "Trainings",
                table: "Exercise",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Exercise_Equipment_EquipmentId",
                schema: "Trainings",
                table: "Exercise",
                column: "EquipmentId",
                principalSchema: "Trainings",
                principalTable: "Equipment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );
        }
    }
}
