using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrackYourLife.Modules.Nutrition.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexToDailyNutritionOverview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_DailyNutritionOverviews_UserId_Date",
                schema: "Nutrition",
                table: "DailyNutritionOverviews",
                columns: new[] { "UserId", "Date" },
                unique: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DailyNutritionOverviews_UserId_Date",
                schema: "Nutrition",
                table: "DailyNutritionOverviews"
            );
        }
    }
}
