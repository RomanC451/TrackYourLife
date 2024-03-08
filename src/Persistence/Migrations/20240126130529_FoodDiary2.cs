using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrackYourLifeDotnet.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FoodDiary2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FullySearched",
                table: "SearchedFood");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "FullySearched",
                table: "SearchedFood",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
