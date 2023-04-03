using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.EntityFrameworkDataAccess.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OutboxMessageConsumers",
                columns: table =>
                    new
                    {
                        Id = table.Column<Guid>(type: "uuid", nullable: false),
                        Name = table.Column<string>(type: "text", nullable: false)
                    },
                constraints: table =>
                    table.PrimaryKey("PK_OutboxMessageConsumers", x => new { x.Id, x.Name })
            );

            migrationBuilder.CreateTable(
                name: "OutboxMessages",
                columns: table =>
                    new
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
                        Error = table.Column<string>(type: "text", nullable: true)
                    },
                constraints: table => table.PrimaryKey("PK_OutboxMessages", x => x.Id)
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "OutboxMessageConsumers");

            migrationBuilder.DropTable(name: "OutboxMessages");
        }
    }
}
