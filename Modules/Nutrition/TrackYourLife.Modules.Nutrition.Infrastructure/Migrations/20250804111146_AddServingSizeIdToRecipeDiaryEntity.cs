using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrackYourLife.Modules.Nutrition.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddServingSizeIdToRecipeDiaryEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ServingSizeId",
                schema: "Nutrition",
                table: "RecipeDiary",
                type: "uuid",
                nullable: false,
                defaultValue: Guid.Empty
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ServingSizeId",
                schema: "Nutrition",
                table: "RecipeDiary"
            );
        }
    }
}
