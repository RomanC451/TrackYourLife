using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrackYourLife.Modules.Nutrition.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRecipeVersioning3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "Version", schema: "Nutrition", table: "Recipe");

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                schema: "Nutrition",
                table: "Recipe",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "xmin", schema: "Nutrition", table: "Recipe");

            migrationBuilder.AddColumn<byte[]>(
                name: "Version",
                schema: "Nutrition",
                table: "Recipe",
                type: "bytea",
                rowVersion: true,
                nullable: false,
                defaultValueSql: "gen_random_bytes(8)"
            );
        }
    }
}
