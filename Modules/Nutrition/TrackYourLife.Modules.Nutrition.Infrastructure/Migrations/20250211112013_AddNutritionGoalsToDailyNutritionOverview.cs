using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrackYourLife.Modules.Nutrition.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNutritionGoalsToDailyNutritionOverview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "CaloriesGoal",
                schema: "Nutrition",
                table: "DailyNutritionOverviews",
                type: "real",
                nullable: false,
                defaultValue: 0f
            );

            migrationBuilder.AddColumn<float>(
                name: "CarbohydratesGoal",
                schema: "Nutrition",
                table: "DailyNutritionOverviews",
                type: "real",
                nullable: false,
                defaultValue: 0f
            );

            migrationBuilder.AddColumn<float>(
                name: "FatGoal",
                schema: "Nutrition",
                table: "DailyNutritionOverviews",
                type: "real",
                nullable: false,
                defaultValue: 0f
            );

            migrationBuilder.AddColumn<float>(
                name: "ProteinGoal",
                schema: "Nutrition",
                table: "DailyNutritionOverviews",
                type: "real",
                nullable: false,
                defaultValue: 0f
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CaloriesGoal",
                schema: "Nutrition",
                table: "DailyNutritionOverviews"
            );

            migrationBuilder.DropColumn(
                name: "CarbohydratesGoal",
                schema: "Nutrition",
                table: "DailyNutritionOverviews"
            );

            migrationBuilder.DropColumn(
                name: "FatGoal",
                schema: "Nutrition",
                table: "DailyNutritionOverviews"
            );

            migrationBuilder.DropColumn(
                name: "ProteinGoal",
                schema: "Nutrition",
                table: "DailyNutritionOverviews"
            );
        }
    }
}
