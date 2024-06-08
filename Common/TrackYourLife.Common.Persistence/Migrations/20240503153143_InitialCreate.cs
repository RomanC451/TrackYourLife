using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrackYourLife.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Food",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    BrandName = table.Column<string>(type: "text", nullable: false, defaultValue: ""),
                    CountryCode = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    NutritionalContent_Calcium = table.Column<double>(type: "double precision", nullable: false),
                    NutritionalContent_Carbohydrates = table.Column<double>(type: "double precision", nullable: false),
                    NutritionalContent_Cholesterol = table.Column<double>(type: "double precision", nullable: false),
                    NutritionalContent_Fat = table.Column<double>(type: "double precision", nullable: false),
                    NutritionalContent_Fiber = table.Column<double>(type: "double precision", nullable: false),
                    NutritionalContent_Iron = table.Column<double>(type: "double precision", nullable: false),
                    NutritionalContent_MonounsaturatedFat = table.Column<double>(type: "double precision", nullable: false),
                    NutritionalContent_NetCarbs = table.Column<double>(type: "double precision", nullable: false),
                    NutritionalContent_PolyunsaturatedFat = table.Column<double>(type: "double precision", nullable: false),
                    NutritionalContent_Potassium = table.Column<double>(type: "double precision", nullable: false),
                    NutritionalContent_Protein = table.Column<double>(type: "double precision", nullable: false),
                    NutritionalContent_SaturatedFat = table.Column<double>(type: "double precision", nullable: false),
                    NutritionalContent_Sodium = table.Column<double>(type: "double precision", nullable: false),
                    NutritionalContent_Sugar = table.Column<double>(type: "double precision", nullable: false),
                    NutritionalContent_TransFat = table.Column<double>(type: "double precision", nullable: false),
                    NutritionalContent_VitaminA = table.Column<double>(type: "double precision", nullable: false),
                    NutritionalContent_VitaminC = table.Column<double>(type: "double precision", nullable: false),
                    NutritionalContent_Energy_Unit = table.Column<string>(type: "text", nullable: false),
                    NutritionalContent_Energy_Value = table.Column<float>(type: "real", nullable: false),
                    ApiId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Food", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OutboxMessageConsumers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxMessageConsumers", x => new { x.Id, x.Name });
                });

            migrationBuilder.CreateTable(
                name: "OutboxMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    OccurredOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProcessedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Error = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxMessages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SearchedFood",
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
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NutritionMultiplier = table.Column<double>(type: "double precision", nullable: false),
                    Unit = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<double>(type: "double precision", nullable: false),
                    ApiId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServingSize", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    CreatedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    VerifiedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FoodServingSize",
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
                        principalTable: "Food",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FoodServingSize_ServingSize_ServingSizeId",
                        column: x => x.ServingSizeId,
                        principalTable: "ServingSize",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FoodDiary",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    FoodId = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<float>(type: "real", nullable: false),
                    ServingSizeId = table.Column<Guid>(type: "uuid", nullable: false),
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
                        principalTable: "Food",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FoodDiary_ServingSize_ServingSizeId",
                        column: x => x.ServingSizeId,
                        principalTable: "ServingSize",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FoodDiary_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserGoals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<int>(type: "integer", nullable: false),
                    PerPeriod = table.Column<string>(type: "text", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserGoals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserGoals_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Food_ApiId",
                table: "Food",
                column: "ApiId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FoodDiary_FoodId",
                table: "FoodDiary",
                column: "FoodId");

            migrationBuilder.CreateIndex(
                name: "IX_FoodDiary_ServingSizeId",
                table: "FoodDiary",
                column: "ServingSizeId");

            migrationBuilder.CreateIndex(
                name: "IX_FoodDiary_UserId",
                table: "FoodDiary",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_FoodServingSize_ServingSizeId",
                table: "FoodServingSize",
                column: "ServingSizeId");

            migrationBuilder.CreateIndex(
                name: "IX_ServingSize_ApiId",
                table: "ServingSize",
                column: "ApiId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserGoals_UserId",
                table: "UserGoals",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserTokens_UserId",
                table: "UserTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserTokens_Value",
                table: "UserTokens",
                column: "Value",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FoodDiary");

            migrationBuilder.DropTable(
                name: "FoodServingSize");

            migrationBuilder.DropTable(
                name: "OutboxMessageConsumers");

            migrationBuilder.DropTable(
                name: "OutboxMessages");

            migrationBuilder.DropTable(
                name: "SearchedFood");

            migrationBuilder.DropTable(
                name: "UserGoals");

            migrationBuilder.DropTable(
                name: "UserTokens");

            migrationBuilder.DropTable(
                name: "Food");

            migrationBuilder.DropTable(
                name: "ServingSize");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
