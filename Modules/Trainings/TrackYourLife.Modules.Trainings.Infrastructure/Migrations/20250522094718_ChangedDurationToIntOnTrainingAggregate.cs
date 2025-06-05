using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrackYourLife.Modules.Trainings.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangedDurationToIntOnTrainingAggregate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"
                ALTER TABLE ""Trainings"".""Training""
                ALTER COLUMN ""Duration"" TYPE integer
                USING EXTRACT(EPOCH FROM ""Duration"")::integer / 60;
            "
            );

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "Trainings",
                table: "Training",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"
                ALTER TABLE ""Trainings"".""Training""
                ALTER COLUMN ""Duration"" TYPE interval
                USING (""Duration"" * interval '1 minute');
            "
            );

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "Trainings",
                table: "Training",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true
            );
        }
    }
}
