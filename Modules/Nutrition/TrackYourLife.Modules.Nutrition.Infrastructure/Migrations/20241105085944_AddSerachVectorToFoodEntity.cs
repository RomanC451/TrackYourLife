using Microsoft.EntityFrameworkCore.Migrations;
using NpgsqlTypes;

#nullable disable

namespace TrackYourLife.Modules.Nutrition.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSerachVectorToFoodEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder
                .AddColumn<NpgsqlTsVector>(
                    name: "SearchVector",
                    schema: "Nutrition",
                    table: "Food",
                    type: "tsvector",
                    nullable: false
                )
                .Annotation("Npgsql:TsVectorConfig", "english")
                .Annotation("Npgsql:TsVectorProperties", new[] { "Name", "BrandName" });

            migrationBuilder
                .CreateIndex(
                    name: "IX_Food_SearchVector",
                    schema: "Nutrition",
                    table: "Food",
                    column: "SearchVector"
                )
                .Annotation("Npgsql:IndexMethod", "GIN");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Food_SearchVector",
                schema: "Nutrition",
                table: "Food"
            );

            migrationBuilder.DropColumn(name: "SearchVector", schema: "Nutrition", table: "Food");
        }
    }
}
