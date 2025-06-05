using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrackYourLife.Modules.Nutrition.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameDailyNutritionOverviewTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DailyNutritionOverviews",
                schema: "Nutrition",
                table: "DailyNutritionOverviews");

            migrationBuilder.RenameTable(
                name: "DailyNutritionOverviews",
                schema: "Nutrition",
                newName: "DailyNutritionOverview",
                newSchema: "Nutrition");

            migrationBuilder.RenameIndex(
                name: "IX_DailyNutritionOverviews_UserId_Date",
                schema: "Nutrition",
                table: "DailyNutritionOverview",
                newName: "IX_DailyNutritionOverview_UserId_Date");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DailyNutritionOverview",
                schema: "Nutrition",
                table: "DailyNutritionOverview",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DailyNutritionOverview",
                schema: "Nutrition",
                table: "DailyNutritionOverview");

            migrationBuilder.RenameTable(
                name: "DailyNutritionOverview",
                schema: "Nutrition",
                newName: "DailyNutritionOverviews",
                newSchema: "Nutrition");

            migrationBuilder.RenameIndex(
                name: "IX_DailyNutritionOverview_UserId_Date",
                schema: "Nutrition",
                table: "DailyNutritionOverviews",
                newName: "IX_DailyNutritionOverviews_UserId_Date");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DailyNutritionOverviews",
                schema: "Nutrition",
                table: "DailyNutritionOverviews",
                column: "Id");
        }
    }
}
