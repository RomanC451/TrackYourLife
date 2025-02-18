using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrackYourLife.Modules.Nutrition.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDailyNutritionOverview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DailyNutritionOverviews",
                schema: "Nutrition",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    NutritionalContent_Calcium = table.Column<float>(type: "real", nullable: false),
                    NutritionalContent_Carbohydrates = table.Column<float>(type: "real", nullable: false),
                    NutritionalContent_Cholesterol = table.Column<float>(type: "real", nullable: false),
                    NutritionalContent_Fat = table.Column<float>(type: "real", nullable: false),
                    NutritionalContent_Fiber = table.Column<float>(type: "real", nullable: false),
                    NutritionalContent_Iron = table.Column<float>(type: "real", nullable: false),
                    NutritionalContent_MonounsaturatedFat = table.Column<float>(type: "real", nullable: false),
                    NutritionalContent_NetCarbs = table.Column<float>(type: "real", nullable: false),
                    NutritionalContent_PolyunsaturatedFat = table.Column<float>(type: "real", nullable: false),
                    NutritionalContent_Potassium = table.Column<float>(type: "real", nullable: false),
                    NutritionalContent_Protein = table.Column<float>(type: "real", nullable: false),
                    NutritionalContent_SaturatedFat = table.Column<float>(type: "real", nullable: false),
                    NutritionalContent_Sodium = table.Column<float>(type: "real", nullable: false),
                    NutritionalContent_Sugar = table.Column<float>(type: "real", nullable: false),
                    NutritionalContent_TransFat = table.Column<float>(type: "real", nullable: false),
                    NutritionalContent_VitaminA = table.Column<float>(type: "real", nullable: false),
                    NutritionalContent_VitaminC = table.Column<float>(type: "real", nullable: false),
                    NutritionalContent_Energy_Unit = table.Column<string>(type: "text", nullable: false),
                    NutritionalContent_Energy_Value = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyNutritionOverviews", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DailyNutritionOverviews",
                schema: "Nutrition");
        }
    }
}
