using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrackYourLife.Modules.Nutrition.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Nutrition");

            migrationBuilder.CreateTable(
                name: "Food",
                schema: "Nutrition",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    BrandName = table.Column<string>(type: "text", nullable: false, defaultValue: ""),
                    CountryCode = table.Column<string>(type: "text", nullable: false),
                    ApiId = table.Column<long>(type: "bigint", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false),
                    NutritionalContents_Calcium = table.Column<float>(type: "real", nullable: false),
                    NutritionalContents_Carbohydrates = table.Column<float>(type: "real", nullable: false),
                    NutritionalContents_Cholesterol = table.Column<float>(type: "real", nullable: false),
                    NutritionalContents_Fat = table.Column<float>(type: "real", nullable: false),
                    NutritionalContents_Fiber = table.Column<float>(type: "real", nullable: false),
                    NutritionalContents_Iron = table.Column<float>(type: "real", nullable: false),
                    NutritionalContents_MonounsaturatedFat = table.Column<float>(type: "real", nullable: false),
                    NutritionalContents_NetCarbs = table.Column<float>(type: "real", nullable: false),
                    NutritionalContents_PolyunsaturatedFat = table.Column<float>(type: "real", nullable: false),
                    NutritionalContents_Potassium = table.Column<float>(type: "real", nullable: false),
                    NutritionalContents_Protein = table.Column<float>(type: "real", nullable: false),
                    NutritionalContents_SaturatedFat = table.Column<float>(type: "real", nullable: false),
                    NutritionalContents_Sodium = table.Column<float>(type: "real", nullable: false),
                    NutritionalContents_Sugar = table.Column<float>(type: "real", nullable: false),
                    NutritionalContents_TransFat = table.Column<float>(type: "real", nullable: false),
                    NutritionalContents_VitaminA = table.Column<float>(type: "real", nullable: false),
                    NutritionalContents_VitaminC = table.Column<float>(type: "real", nullable: false),
                    NutritionalContents_Energy_Unit = table.Column<string>(type: "text", nullable: false),
                    NutritionalContents_Energy_Value = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Food", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Recipe",
                schema: "Nutrition",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IsOld = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    NutritionalContents_Calcium = table.Column<float>(type: "real", nullable: false),
                    NutritionalContents_Carbohydrates = table.Column<float>(type: "real", nullable: false),
                    NutritionalContents_Cholesterol = table.Column<float>(type: "real", nullable: false),
                    NutritionalContents_Fat = table.Column<float>(type: "real", nullable: false),
                    NutritionalContents_Fiber = table.Column<float>(type: "real", nullable: false),
                    NutritionalContents_Iron = table.Column<float>(type: "real", nullable: false),
                    NutritionalContents_MonounsaturatedFat = table.Column<float>(type: "real", nullable: false),
                    NutritionalContents_NetCarbs = table.Column<float>(type: "real", nullable: false),
                    NutritionalContents_PolyunsaturatedFat = table.Column<float>(type: "real", nullable: false),
                    NutritionalContents_Potassium = table.Column<float>(type: "real", nullable: false),
                    NutritionalContents_Protein = table.Column<float>(type: "real", nullable: false),
                    NutritionalContents_SaturatedFat = table.Column<float>(type: "real", nullable: false),
                    NutritionalContents_Sodium = table.Column<float>(type: "real", nullable: false),
                    NutritionalContents_Sugar = table.Column<float>(type: "real", nullable: false),
                    NutritionalContents_TransFat = table.Column<float>(type: "real", nullable: false),
                    NutritionalContents_VitaminA = table.Column<float>(type: "real", nullable: false),
                    NutritionalContents_VitaminC = table.Column<float>(type: "real", nullable: false),
                    NutritionalContents_Energy_Unit = table.Column<string>(type: "text", nullable: false),
                    NutritionalContents_Energy_Value = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recipe", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SearchedFood",
                schema: "Nutrition",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SearchedFood", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServingSize",
                schema: "Nutrition",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NutritionMultiplier = table.Column<float>(type: "real", nullable: false),
                    Unit = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<float>(type: "real", nullable: false),
                    ApiId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServingSize", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RecipeDiary",
                schema: "Nutrition",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RecipeId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<float>(type: "real", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    MealType = table.Column<string>(type: "text", nullable: false),
                    CreatedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipeDiary", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecipeDiary_Recipe_RecipeId",
                        column: x => x.RecipeId,
                        principalSchema: "Nutrition",
                        principalTable: "Recipe",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FoodDiary",
                schema: "Nutrition",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FoodId = table.Column<Guid>(type: "uuid", nullable: false),
                    ServingSizeId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<float>(type: "real", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    MealType = table.Column<string>(type: "text", nullable: false),
                    CreatedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodDiary", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FoodDiary_Food_FoodId",
                        column: x => x.FoodId,
                        principalSchema: "Nutrition",
                        principalTable: "Food",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FoodDiary_ServingSize_ServingSizeId",
                        column: x => x.ServingSizeId,
                        principalSchema: "Nutrition",
                        principalTable: "ServingSize",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FoodServingSize",
                schema: "Nutrition",
                columns: table => new
                {
                    FoodId = table.Column<Guid>(type: "uuid", nullable: false),
                    ServingSizeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Index = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodServingSize", x => new { x.FoodId, x.ServingSizeId });
                    table.ForeignKey(
                        name: "FK_FoodServingSize_Food_FoodId",
                        column: x => x.FoodId,
                        principalSchema: "Nutrition",
                        principalTable: "Food",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FoodServingSize_ServingSize_ServingSizeId",
                        column: x => x.ServingSizeId,
                        principalSchema: "Nutrition",
                        principalTable: "ServingSize",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ingredient",
                schema: "Nutrition",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FoodId = table.Column<Guid>(type: "uuid", nullable: false),
                    ServingSizeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<float>(type: "real", nullable: false),
                    RecipeId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ingredient", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ingredient_Food_FoodId",
                        column: x => x.FoodId,
                        principalSchema: "Nutrition",
                        principalTable: "Food",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ingredient_Recipe_RecipeId",
                        column: x => x.RecipeId,
                        principalSchema: "Nutrition",
                        principalTable: "Recipe",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ingredient_ServingSize_ServingSizeId",
                        column: x => x.ServingSizeId,
                        principalSchema: "Nutrition",
                        principalTable: "ServingSize",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Food_ApiId",
                schema: "Nutrition",
                table: "Food",
                column: "ApiId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FoodDiary_FoodId",
                schema: "Nutrition",
                table: "FoodDiary",
                column: "FoodId");

            migrationBuilder.CreateIndex(
                name: "IX_FoodDiary_ServingSizeId",
                schema: "Nutrition",
                table: "FoodDiary",
                column: "ServingSizeId");

            migrationBuilder.CreateIndex(
                name: "IX_FoodServingSize_ServingSizeId",
                schema: "Nutrition",
                table: "FoodServingSize",
                column: "ServingSizeId");

            migrationBuilder.CreateIndex(
                name: "IX_Ingredient_FoodId",
                schema: "Nutrition",
                table: "Ingredient",
                column: "FoodId");

            migrationBuilder.CreateIndex(
                name: "IX_Ingredient_RecipeId",
                schema: "Nutrition",
                table: "Ingredient",
                column: "RecipeId");

            migrationBuilder.CreateIndex(
                name: "IX_Ingredient_ServingSizeId",
                schema: "Nutrition",
                table: "Ingredient",
                column: "ServingSizeId");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeDiary_RecipeId",
                schema: "Nutrition",
                table: "RecipeDiary",
                column: "RecipeId");

            migrationBuilder.CreateIndex(
                name: "IX_ServingSize_ApiId",
                schema: "Nutrition",
                table: "ServingSize",
                column: "ApiId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FoodDiary",
                schema: "Nutrition");

            migrationBuilder.DropTable(
                name: "FoodServingSize",
                schema: "Nutrition");

            migrationBuilder.DropTable(
                name: "Ingredient",
                schema: "Nutrition");

            migrationBuilder.DropTable(
                name: "RecipeDiary",
                schema: "Nutrition");

            migrationBuilder.DropTable(
                name: "SearchedFood",
                schema: "Nutrition");

            migrationBuilder.DropTable(
                name: "Food",
                schema: "Nutrition");

            migrationBuilder.DropTable(
                name: "ServingSize",
                schema: "Nutrition");

            migrationBuilder.DropTable(
                name: "Recipe",
                schema: "Nutrition");
        }
    }
}
