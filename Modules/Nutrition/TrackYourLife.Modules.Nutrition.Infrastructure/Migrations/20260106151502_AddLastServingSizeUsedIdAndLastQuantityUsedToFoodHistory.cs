using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrackYourLife.Modules.Nutrition.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLastServingSizeUsedIdAndLastQuantityUsedToFoodHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "LastQuantityUsed",
                schema: "Nutrition",
                table: "FoodHistory",
                type: "real",
                nullable: false,
                defaultValue: 0f
            );

            migrationBuilder.AddColumn<Guid>(
                name: "LastServingSizeUsedId",
                schema: "Nutrition",
                table: "FoodHistory",
                type: "uuid",
                nullable: false,
                defaultValue: Guid.Empty
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastQuantityUsed",
                schema: "Nutrition",
                table: "FoodHistory"
            );

            migrationBuilder.DropColumn(
                name: "LastServingSizeUsedId",
                schema: "Nutrition",
                table: "FoodHistory"
            );
        }
    }
}
