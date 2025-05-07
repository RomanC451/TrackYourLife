using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrackYourLife.Modules.Nutrition.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFoodHistoryAndOutboxTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FoodHistory",
                schema: "Nutrition",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    FoodId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUsedAt = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FoodHistory_Food_FoodId",
                        column: x => x.FoodId,
                        principalSchema: "Nutrition",
                        principalTable: "Food",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "OutboxMessageConsumers",
                schema: "Nutrition",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxMessageConsumers", x => new { x.Id, x.Name });
                }
            );

            migrationBuilder.CreateTable(
                name: "OutboxMessages",
                schema: "Nutrition",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    OccurredOnUtc = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    ProcessedOnUtc = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    Error = table.Column<string>(type: "text", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxMessages", x => x.Id);
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_FoodHistory_FoodId",
                schema: "Nutrition",
                table: "FoodHistory",
                column: "FoodId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_FoodHistory_LastUsedAt",
                schema: "Nutrition",
                table: "FoodHistory",
                column: "LastUsedAt"
            );

            migrationBuilder.CreateIndex(
                name: "IX_FoodHistory_UserId_FoodId",
                schema: "Nutrition",
                table: "FoodHistory",
                columns: new[] { "UserId", "FoodId" },
                unique: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "FoodHistory", schema: "Nutrition");

            migrationBuilder.DropTable(name: "OutboxMessageConsumers", schema: "Nutrition");

            migrationBuilder.DropTable(name: "OutboxMessages", schema: "Nutrition");
        }
    }
}
