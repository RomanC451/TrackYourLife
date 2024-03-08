using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrackYourLifeDotnet.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Initial2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ServingSize_ApiId",
                table: "ServingSize",
                column: "ApiId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Food_ApiId",
                table: "Food",
                column: "ApiId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ServingSize_ApiId",
                table: "ServingSize");

            migrationBuilder.DropIndex(
                name: "IX_Food_ApiId",
                table: "Food");
        }
    }
}
