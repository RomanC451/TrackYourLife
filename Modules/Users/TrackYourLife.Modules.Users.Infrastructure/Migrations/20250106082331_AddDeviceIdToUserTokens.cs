using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrackYourLife.Modules.Users.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDeviceIdToUserTokens : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DeviceId",
                schema: "Users",
                table: "Tokens",
                type: "uuid",
                nullable: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_Tokens_DeviceId",
                schema: "Users",
                table: "Tokens",
                column: "DeviceId"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tokens_DeviceId",
                schema: "Users",
                table: "Tokens"
            );

            migrationBuilder.DropColumn(name: "DeviceId", schema: "Users", table: "Tokens");
        }
    }
}
